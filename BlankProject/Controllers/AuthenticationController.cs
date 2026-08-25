using BLL;
using BLL.FajrLog;
using BLL.Interface;
using DTO.Entities.MaxaRabbitMQ;
using DTO.User;
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


    public AuthenticationController(IAuthManager _AuthManager, IUserLogManager _UserLogManager,
        IConfiguration configuration,
        IRedisManager _Redis, IConstantManager constantManager, IFajrLogManager fajrLogManager = null
        ) : base()
    {
        _configuration = configuration;
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
            var User = Session.GetUser();
            if (User != null)
                return RedirectToAction("index", "Dashboard", new { area = "Admin" });
        }

        //var showCaptcha = HttpContext.GetCookieShowCaptcha();
        //if (showCaptcha)
        //    ViewBag.captcha = true;

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
        var res = AuthManager.Login(Mobile, Password);
        var User = res.Model as UserSessionDTO;
        if (!res.Status)
        {
            await Redis.db.SetLoginLog(Redis.ContextAccessor, fajrActionType, Mobile, User?.FullName, User?.Id, false, res.Message);
            if (User == null)
            {
                ViewBag.Error = res.Message;
                ViewBag.InvalidLogin = true;

                return View();
            }
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

        if (!string.IsNullOrEmpty(RetUrl))
            return Redirect(RetUrl);

        return RedirectToAction("index", "Dashboard", new { area = "Admin" });
    }
    #endregion

    #region ورود با چهره
    public IActionResult LoginPerson()
    {
        // اگر پیغامی در TempData وجود داشت به ViewData منتقل می‌کنیم تا در View نمایش داده شود
        if (TempData["Message"] != null)
            ViewData["Message"] = TempData["Message"].ToString();

        return View();
    }



    #endregion

    #region ZP

    [HttpGet]
    public IActionResult IndexZP(long? mid)
    {
        ViewBag.ApiBaseUrl = _configuration["ApiAddress:Refit"]?.TrimEnd('/');

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
        if (kioskId <= 0 || enrollId <= 0)
        {
            TempData["Message"] = "اطلاعات ورود معتبر نیست.";
            return RedirectToAction("IndexZP");
        }

        var user = AuthManager.LoginWithFaceZP(enrollId);

        if (user == null)
        {
            TempData["Message"] = "کاربر یافت نشد. لطفاً دوباره تلاش کنید.";
            return RedirectToAction("IndexZP");
        }

        var token = await Redis.db.SetLoginToken(user.Id);
        HttpContext.SetCookieUserToken(token);
        HttpContext.Session.SetUser(user);

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }
    #endregion

    #region خروج از حساب کاربری - logout
    public async Task<ActionResult> Logout()
    {
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