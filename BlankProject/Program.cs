using BLL.Interface;
using BLL;
using Filters;
using Infrastructure.Data;
using AntiXssMiddleware.Middleware;
using Services.RedisService;
using ITOWebApiClient;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using BLL.LogSystem.UserChangeLogEvent;
using System.Net;
using BLL.FajrLog;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Utf8Json;
using StackExchange.Redis.Extensions.Newtonsoft;
using BLL.FoodManag.DiningHallBL;
using BLL.FoodManag.FoodBL;
using BLL.FoodManag.FoodTypesBl;
using BLL.FoodManag.KitchenBL;
using BLL.Garrision;
using BLL.ReserveManagment;
using Food.Hubs;
using Food.Hubs.ZP;
using DTO.Base;

internal class Program
{
    private static void Main(string[] args)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        var builder = WebApplication.CreateBuilder(args);

        #region App Setting Rabbit Mq
        // بارگذاری کانفیگ
        // var busConfig = builder.Configuration.GetSection("BusConfig").Get<DTO.Entities.BusConfigDTO>();

        #endregion

        #region Serilog

        //builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
        #endregion

        // Add services to the container.
        var services = builder.Services;
        services.Configure<PrinterOptions>(
    builder.Configuration.GetSection("Printer"));


        services.AddControllersWithViews()
                        .AddRazorRuntimeCompilation()
                        .AddNewtonsoftJson(options =>
                            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        );

        services.AddRazorPages();

        #region DB Context
        // Main DB Connection String
        var connectionString = builder.Configuration.GetConnectionString("ApplicationContext"); // خوندن از appsetting
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:ApplicationContext is missing/empty.");

        //var connectionString = builder.Configuration["ConnectionStrings:ApplicationContext"]; // خوندن از user secret
        services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString, sqlServerOptions =>
        {
            sqlServerOptions.CommandTimeout(3600);
            sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
        }));

        // Log DB Connection String
        var logConnectionString = builder.Configuration.GetConnectionString("LogContext"); // خوندن از appsetting
        if (string.IsNullOrWhiteSpace(logConnectionString))
            throw new InvalidOperationException("ConnectionStrings:LogContext is missing/empty.");                                                                     //var logConnectionString = builder.Configuration["ConnectionStrings:LogContext"]; // خوندن از user secret
        services.AddDbContext<LogContext>(options => options.UseSqlServer(logConnectionString));

        #endregion



        #region redis
        //var multiplexer = ConnectionMultiplexer.Connect("localhost:6379");
        //services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        var RedisConfigurations = new List<RedisConfiguration>();
        var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisConfiguration>();
        RedisConfigurations.Add(redisConfig);

        services.AddStackExchangeRedisExtensions<Utf8JsonSerializer>((options) =>
        {
            return RedisConfigurations;
        }).AddStackExchangeRedisExtensions<NewtonsoftSerializer>((options) =>
        {
            return RedisConfigurations;
        });

        services.AddSingleton<IRedisManager, RedisManager>();

        #region ثبت لاگ در بکگراند
        services.AddHostedService<LogBackgroundWorker>();
        #endregion
        #endregion

        #region گرفتن httpcontext در کلاس لایبرری ها
        services.AddHttpContextAccessor();
        #endregion


        #region اضافه کردن سرویس کوکی
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
        });
        #endregion


        #region اضافه کردن سرویس سشن
        // services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<double?>("Setting:SessionTimeout") ?? 60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Name = "_session.cookie";
        });
        #endregion


        #region اضافه کردن سرویس کش
        services.AddMemoryCache();

        services.AddResponseCaching(options =>
        {
            options.UseCaseSensitivePaths = false;
        });
        #endregion



        #region فیلتر برای اکشن ها
        // فیلتر بررسی لاگین ادمین در پنل ادمین
        services.AddScoped(_ => new UserAuthorize());
        #endregion



        #region نام کوکی و هدر AntiForgeryKey برای جلوگیری از حملات csrf
        services.AddAntiforgery(options =>
        {
            options.Cookie.Name = "_CSRF.cookie";
            options.HeaderName = "_CSRF_header";
        });



        // services.AddMvc(options => options.EnableEndpointRouting = false);



        //// If using IIS:
        services.Configure<IISServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
        #endregion




        #region Dipendency Injection
        // موجودیت های پایه
        services.AddScoped<DbContext, ApplicationContext>();

        // برای لاگ گیری
        services.AddScoped<DbContext, LogContext>();

        // فکتوری برای همه کانتکست ها
        services.AddScoped<DbContexts>();

        #region DataTable
        services.AddScoped<IDataTableManager, DataTableManager>();
        #endregion


        #region AuthSystem
        services.AddScoped<IAuthManager, AuthManager>();
        services.AddScoped<IMenuManager, MenuManager>();
        services.AddScoped<IRoleManager, RoleManager>();
        services.AddScoped<IRoleMenuManager, RoleMenuManager>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IUserPasswordHistoryManager, UserPasswordHistoryManager>();
        services.AddScoped<IReservationUserProvisioningManager, ReservationUserProvisioningManager>();
        #endregion


        #region LogSystem
        services.AddScoped<IUserLogManager, UserLogManager>();
        #endregion


        #region Shared
        services.AddScoped<IConstantManager, ConstantManager>();
        #endregion

        #region Food


        #region FoodPlan
        services.AddScoped<IFoodPlanManager, FoodPlanManager>();
        services.AddScoped<IFoodPlanDayManager, FoodPlanDayManager>();
        services.AddScoped<IFoodSourceManager, FoodSourceManager>();
        services.AddScoped<IFoodManager, FoodManager>();
        services.AddScoped<IFoodTypesManager, FoodTypesManager>();
        services.AddScoped<IKitchensManager, KitchensManager>();
        services.AddScoped<IDiningHallManager, DiningHallManager>();
        #endregion

        #endregion

        #region Qouta
        services.AddScoped<IQoutaPersonManager, QoutaPersonManager>();
        services.AddScoped<IUnitQuotaManager, UnitQuotaManager>();

        #endregion

        #region Statistic
        services.AddScoped<IStatisticManager, StatisticManager>();
        services.AddScoped<IUnitStatisticManager, UnitStatisticManager>();

        #endregion

        #region UnitCalendarInfo
        services.AddScoped<IUnitCalendarManager, UnitCalendarManager>();

        #endregion

        #region Extera Food

        services.AddScoped<IGuestExtraFoodRequestManager, GuestExtraFoodRequestManager>();

        #endregion

        #region Extera Food Person

        services.AddScoped<IExtraQuotaPersonManager, ExtraQuotaPersonManager>();

        #endregion

        #region PersonalType

        services.AddScoped<IPersonManager, PersonManager>();

        services.AddScoped<IPersonalManager, PerssonalManager>();

        #endregion

        #region Meal
        services.AddScoped<IMealManager, MealManager>();

        #endregion

        #region Day
        services.AddScoped<IFoodPlanManager, FoodPlanManager>();

        #endregion

        #region Years
        services.AddScoped<IYearsManager, YearsManager>();

        #endregion

        #region Rabbit MQ
        //  builder.Services.AddSingleton(busConfig); // ❌ خیلی مهم
        // services.AddScoped<IRabbitMqConsumerManager, RabbitMqConsumerManager>();
        // services.AddScoped<IRabbitMqPublisherManager, RabbitMqPublisherManager>();
        // services.AddSingleton<IRabbitMqPublisherManager, RabbitMqPublisherManager>();
        // builder.Services.AddHostedService<RabbitMqBackgroundConsumer>();


        #endregion


        #region API

        services.AddScoped<IWebApiManager, WebApiManager>();

        services.AddScoped<IUserChangeLogEventManager, UserChangeLogEventManager>();

        services.AddScoped<IFajrLogManager, FajrLogManager>();





        #endregion

        #region FoodRserve FoodReserveDetail

        services.AddScoped<IFoodReserveManager, FoodReserveManager>();

        services.AddScoped<IFoodReserveDetailManager, FoodReserveDetailManager>();

        #endregion
        #region ZP
        services.AddScoped<KioskHubService>();

        #endregion
        #endregion

        #region Serilog
        // services.AddScoped<ISeriLogManager, SeriLogManager>();
        #endregion



        #region MyRegion
        #endregion

        services.AddSingleton<ApiTokenCacheClient>();
        services.AddHttpClient();
        services.AddDistributedMemoryCache();
        services.AddSignalR();

        builder.Services.AddCors(options =>
options.AddPolicy("AllowAllOrigins",
    builder => builder.AllowAnyOrigin()
        .AllowAnyHeader()
    .AllowAnyMethod()
    ));

        //builder.Services.AddCors(options =>
        //{
        //    options.AddPolicy("KioskCors", policy =>
        //    {
        //        policy
        //            .WithOrigins("https://localhost:44340")   // ✅ همون origin که تو خطا اومده
        //            .AllowAnyHeader()
        //            .AllowAnyMethod()
        //            .AllowCredentials();
        //    });
        //});


        var app = builder.Build();


        app.UseDeveloperExceptionPage();
       // app.UseHsts();
        // app.UseMvc();
        //app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy(new CookiePolicyOptions
        { Secure = CookieSecurePolicy.None, 
            HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always });

        app.UseRouting();
        // app.UseCors("KioskCors");
        app.UseCors("AllowAllOrigins");

        app.UseSession();

        app.UseAuthorization();


        #region امنیت
        // برای جلوگیری از حملات xss
        app.UseAntiXssMiddleware();

        #region تنظیم هدر برای جلوگیری از حملات
        app.Use(async (context, next) =>
        {
            // برای جلوگیری از iframe شدن صفحات سایت و براي مقابله در برابر حملات ClickJacking
            context.Response.Headers.Add("X-Frame-Options", "DENY");

            // جلوگیری از حملات xss
            context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");

            // جلوگیری از MIME-Sniffing و تغییر پسوند فایل ها
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            context.Response.Headers.Remove("Server");
            await next();
        });
        #endregion
        #endregion



        #region Exception Handler
        //app.UseExceptionHandler("/error");
        app.UseExceptionHandler(
                  new ExceptionHandlerOptions()
                  {
                      AllowStatusCode404Response = true,
                      ExceptionHandlingPath = "/error"
                  }
              );
        #endregion

        //#region hangfire

        //app.MapHangfireDashboard();
        //app.UseHangfireDashboard();
        //app.MapHangfireJobs();
        ////BackgroundJob.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

        //#endregion

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            //pattern: "{controller=Authentication}/{action=Index}");
            pattern: "{controller=Authentication}/{action=IndexZP}");

        app.MapHub<AuthHub>("/authHub");


        //#region استیمول سافت
        //app.UseMvc(routes =>app.MapHub<AuthHub>("/authHub");

        //{
        //    routes.MapRoute(
        //        name: "default",
        //    template: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
        //});
        //#endregion
        app.Run();
    }
}

