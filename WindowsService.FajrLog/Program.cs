using BLL.FajrLog;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Services.RedisService;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using StackExchange.Redis.Extensions.Utf8Json;
using WindowsService.FajrLog;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "FajrLogService";
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();


        #region DB Context
        var connectionString = hostContext.Configuration.GetConnectionString("ApplicationContext");
        services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));
        #endregion



        #region گرفتن httpcontext در کلاس لایبرری ها
        services.AddHttpContextAccessor();
        #endregion


        #region redis
        //var multiplexer = ConnectionMultiplexer.Connect("localhost:6379");
        //services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        var RedisConfigurations = new List<RedisConfiguration>();
        var redisConfig = hostContext.Configuration.GetSection("Redis").Get<RedisConfiguration>();
        RedisConfigurations.Add(redisConfig);

        services.AddStackExchangeRedisExtensions<Utf8JsonSerializer>((options) =>
        {
            return RedisConfigurations;
        }).AddStackExchangeRedisExtensions<NewtonsoftSerializer>((options) =>
        {
            return RedisConfigurations;
        });

        services.AddSingleton<IRedisManager, RedisManager>();
        #endregion


        #region ِDipendency Injection
        services.AddScoped<DbContext, ApplicationContext>();
        services.AddScoped<IFajrLogManager, FajrLogManager>();
        services.AddScoped<IRedisManager, RedisManager>();
        #endregion


     

    }).Build();


await host.RunAsync();
