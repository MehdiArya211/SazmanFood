using BLL;
using BLL.FajrLog;
using BLL.Interface;
using DTO.Entities.MaxaRabbitMQ;
using DTO.User;
using ITOWebApiClient;
using FajrLog.Enum;
using Microsoft.AspNetCore.Mvc;
using Services.CookieServices;
using Services.RedisService;
using Services.SessionServices;
using Utilities.Extentions;


namespace Food.Controllers;

/// <summary>
/// صفحه لاگین کاربران با یوزر پس
/// </summary>
public class AuthenticationController : Controller
{
    private readonly IAuthManager AuthManager;
    private readonly IUserLogManager UserLogManager;
    private readonly IConstantManager ConstantManager;
    private readonly IRedisManager Redis;
    private readonly ISession Session;
    private readonly IFajrLogManager FajrLogManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IUserManager UserManager;
    private readonly IWebApiManager WebApiManager;
    private readonly ApiTokenCacheClient ApiTokenClient;


    public AuthenticationController(IAuthManager _AuthManager, IUserLogManager _UserLogManager,
        IConfiguration configuration,
        IRedisManager _Redis,
        IConstantManager constantManager,
        ILogger<AuthenticationController> logger,
        IUserManager userManager,
        IWebApiManager webApiManager,
        ApiTokenCacheClient apiTokenClient,
        IFajrLogManager fajrLogManager = null
        ) : base()
    {
        _configuration = configuration;
        _logger = logger;
        UserManager = userManager;
        WebApiManager = webApiManager;
        ApiTokenClient = apiTokenClient;
        AuthManager = _AuthManager;
        UserLogManager = _UserLogManager;
        Redis = _Redis;
        Session = Redis.ContextAccessor.HttpContext.Session;
        ConstantManager = constantManager;
        FajrLogManager = fajrLogManager;
    }

    #region لاگین به پنل کاربر
    /// <summary>
    /// لاگین
    /// </summary>
    public IActionResult Index(long? mid)
    {
        if (mid == null)
        {
            var user = Session.GetUser();
            if (user != null)
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            // ورود عادی همیشه از صفحه تشخیص چهره آغاز می‌شود.
            return RedirectToAction(nameof(IndexZP));
        }

        // احراز هویت مجدد منوهای حساس مستقیماً فرم نام کاربری را نمایش می‌دهد.
        return View();
    }


    public IActionResult Login()
    {

        //var showCaptcha = HttpContext.GetCookieShowCaptcha();
        //if (showCaptcha)
        //    ViewBag.captcha = true;

        return View("index");
    }

    /// <summary>
    /// لاگین با یوزر پس
    /// </summary>
    /// <param name="Mobile">نام کاربری</param>
    /// <param name="Password">پسورد</param>
    /// <param name="MenuId">اگر لاگین جهت احراز هویت مجدد برای منو خاص بود مقدار دارد</param>
    /// <param name="RetUrl"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Index(string Mobile, string Password, string Captcha, string RetUrl, long? MenuId)
    {

        FajrActionType fajrActionType = MenuId == null ? FajrActionType.logIn : FajrActionType.logInSecurePage;

        #region بررسی تعداد تلاش برای لاگین و قفل بودن حساب کاربری
        //لاگ دفعات تلاش برای لاگین
        var loginLog = await Redis.db.SetLoginLog(Mobile);
        var FailedLoginCount = ConstantManager.GetFailedLoginCount();
        if (loginLog != null && loginLog.Count > FailedLoginCount)
        {
            fajrActionType = FajrActionType.BlockUser;
            var diff = (int)(loginLog.CreateDate.AddMinutes(20) - DateTime.Now).TotalMinutes;
            await Redis.db.SetLoginLog(Redis.ContextAccessor, fajrActionType, Mobile, null, null, false, $"مسدود شدن حساب کاربری تا {diff} دقیقه دیگر به دلیل {loginLog.Count} بار ورود اشتباه کلمه عبور. ");
            ViewBag.Error = $"حساب کاربری شما بدلیل ورود اشتباه کلمه عبور تا {diff} دقیقه آینده مسدود می باشد.";
            return View();
        }
        #endregion

        #region بررسی کپچا در صورت وجود
        // برای دفعات بعدی لاگین در صورت اشتباه وارد کردن کلمه عبور، کپچا نشان داده شود
        //ViewBag.captcha = true;
        //var showCaptcha = HttpContext.GetCookieShowCaptcha();
        //HttpContext.SetCookieShowCaptcha();

        //if (showCaptcha)
        //{
        //    var captcha = HttpContext.Session.GetString("Captcha")?.Trim().ToEnglishNumber();
        //    if (string.IsNullOrEmpty(captcha) || captcha != Captcha)
        //    {
        //        await Redis.db.SetLoginLog(Redis.ContextAccessor, fajrActionType, Mobile, null, null, false, "کد امنیتی صحیح نیست!");
        //        ViewBag.Error = "کد امنیتی صحیح نیست!";
        //        return View();
        //    }
        //}
        #endregion

        #region عملیات لاگین
        Mobile = Mobile?.Trim().ToLower().ToEnglishNumber().ToPersianCharacter();
        Password = Password?.Trim().ToEnglishNumber();

        var res = AuthManager.Login(Mobile, Password);
        var User = res.Model as UserSessionDTO;

        if (!res.Status && User == null && !UserManager.ExistsByUsername(Mobile))
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Mobile) || Password != Mobile)
                {
                    res.Message =
                        "کاربر در سامانه یافت نشد. در اولین ورود، نام کاربری و کلمه عبور باید همان کد پرسنلی باشند.";
                }
                else
                {
                    var accessToken = await ApiTokenClient.GetApiToken(
                        CustomSettings.Instance.ClientId,
                        CustomSettings.Instance.Scope,
                        CustomSettings.Instance.ClientSecret,
                        CustomSettings.Instance.ROPC_UserName,
                        CustomSettings.Instance.ROPC_Password);

                    var person = WebApiManager.GetPersonalByPersonCode(Mobile, accessToken);

                    if (person == null ||
                        string.IsNullOrWhiteSpace(person.personalCode) ||
                        person.personalCode.Trim().ToEnglishNumber() != Mobile)
                    {
                        res.Message = "پرسنلی با این کد پرسنلی در سرویس پرسنلی یافت نشد.";
                    }
                    else
                    {
                        var createResult =
                            UserManager.CreateMealBookerFromPersonnel(person, Password);

                        if (!createResult.Status)
                        {
                            res.Message = createResult.Message;
                        }
                        else
                        {
                            _logger.LogInformation(
                                "کاربر رزروکننده غذا از سرویس پرسنلی ایجاد شد. PersonCode: {PersonCode}",
                                Mobile);

                            res = AuthManager.Login(Mobile, Password);
                            User = res.Model as UserSessionDTO;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "استعلام سرویس پرسنلی یا ساخت خودکار کاربر ناموفق بود. PersonCode: {PersonCode}",
                    Mobile);

                res.Message =
                    "در حال حاضر ارتباط با سرویس پرسنلی برقرار نیست. لطفاً دوباره تلاش کنید.";
            }
        }

        if (!res.Status || User == null)
        {
            await Redis.db.SetLoginLog(
                Redis.ContextAccessor,
                fajrActionType,
                Mobile,
                User?.FullName,
                User?.Id,
                false,
                res.Message);

            ViewBag.Error = res.Message;
            ViewBag.InvalidLogin = true;
            return View();
        }
        #endregion

        #region اطلاعات ردیس و کوکی

        if (MenuId == null)
        {
            // افزودن توکن کاربر به ردیس برای جلوگیری از لاگین همزمان 2 نفر با یک اکانت
            var token = await Redis.db.SetLoginToken(User.Id);

            // افزودن توکن به کوکی
            HttpContext.SetCookieUserToken(token);
        }
        else
        {
            // اگر لاگین جهت احراز هویت مجدد برای منو خاص بود، اطلاعات درون ردیس ذخیره شود
            await Redis.db.SetUserReAuthorizeMenu((int)MenuId, User.Id);
        }
        // حذف اطلاعات مربوط به کنترل تعداد دفعات تلاش برای لاگین
        await Redis.db.RemoveLoginLog(Mobile);
        #endregion

        // کاربری که با کد پرسنلی ساخته شده، پیش از دسترسی به سامانه
        // باید رمز اولیه خود را تغییر دهد.
        if (!User.PasswordIsChanged)
        {
            return RedirectToAction(
                "ChangePassword",
                "Profile",
                new { area = "Admin" });
        }

        // فقط بازگشت به مسیرهای داخلی مجاز است تا از Open Redirect جلوگیری شود.
        if (!string.IsNullOrWhiteSpace(RetUrl) && Url.IsLocalUrl(RetUrl))
            return LocalRedirect(RetUrl);

        return RedirectToAction("index", "Dashboard", new { area = "Admin" });
    }
    #endregion

    #region ورود با چهره
    public IActionResult LoginPerson()
    {
        // مسیر قدیمی برای حفظ سازگاری، به صفحه فعال تشخیص چهره هدایت می‌شود.
        return RedirectToAction(nameof(IndexZP));
    }



    #endregion

    #region ZP

    [HttpGet]
    public IActionResult IndexZP(long? mid)
    {
        var hubBaseUrl = _configuration["ApiAddress:Refit"]?.TrimEnd('/');
        ViewBag.ApiBaseUrl = hubBaseUrl;

        _logger.LogInformation(
            "صفحه ورود بیومتریک باز شد. IP کاربر: {RemoteIp}، آدرس سرویس Hub: {HubBaseUrl}",
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            hubBaseUrl);

        if (mid == null)
        {
            var user = Session.GetUser();
            if (user != null)
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        var showCaptcha = HttpContext.GetCookieShowCaptcha();
        if (showCaptcha)
            ViewBag.captcha = true;

        return View("IndexZP"); // نام فایل ویو
    }

    [HttpGet]
    public async Task<IActionResult> FinalizeFaceLogin(int kioskId, long enrollId)
    {
        _logger.LogInformation(
            "درخواست نهایی‌سازی ورود بیومتریک دریافت شد. KioskId: {KioskId}، EnrollId: {EnrollId}، IP: {RemoteIp}",
            kioskId,
            enrollId,
            HttpContext.Connection.RemoteIpAddress?.ToString());

        if (kioskId <= 0 || enrollId <= 0)
        {
            _logger.LogWarning(
                "ورود بیومتریک رد شد؛ KioskId یا EnrollId معتبر نیست. KioskId: {KioskId}، EnrollId: {EnrollId}",
                kioskId,
                enrollId);

            TempData["Message"] = "اطلاعات ورود معتبر نیست.";
            return RedirectToAction("IndexZP");
        }

        var user = AuthManager.LoginWithFaceZP(enrollId);

        if (user == null)
        {
            _logger.LogWarning(
                "برای EnrollId دریافتی، کاربری یافت نشد. KioskId: {KioskId}، EnrollId: {EnrollId}",
                kioskId,
                enrollId);

            TempData["Message"] = "کاربر یافت نشد. لطفاً دوباره تلاش کنید.";
            return RedirectToAction("IndexZP");
        }

        _logger.LogInformation(
            "کاربر بیومتریک پیدا شد. KioskId: {KioskId}، EnrollId: {EnrollId}، UserId: {UserId}",
            kioskId,
            enrollId,
            user.Id);

        var token = await Redis.db.SetLoginToken(user.Id);
        HttpContext.SetCookieUserToken(token);
        HttpContext.Session.SetUser(user);

        _logger.LogInformation(
            "Session و Cookie ورود بیومتریک ایجاد شد و کاربر به داشبورد منتقل می‌شود. UserId: {UserId}",
            user.Id);

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }
    #endregion

    #region خروج از حساب کاربری - logout
    public async Task<ActionResult> Logout()
    {
        // پیام‌های مربوط به کاربر قبلی نباید در صفحه عمومی تشخیص چهره نمایش داده شوند.
        TempData.Remove("Message");
        TempData.Remove("Error");
        TempData.Remove("Success");

        var user = Session.GetUser();
        if (user != null)
        {

            await Redis.db.RemoveLoginToken(user.Id);

            Session.RemoveUser();

            // پاک کردن کوکی
            HttpContext.Response.Cookies.Delete("_session.cookie"); // توجه: اسم کوکی باید دقیق باشه
        }

        return RedirectToAction("IndexZP", "Authentication");
    }
    #endregion





}