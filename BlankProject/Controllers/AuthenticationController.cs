using BLL;
using BLL.FajrLog;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using BLL.Interface;
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
    private readonly IMemoryCache _faceEventCache;
    private readonly IReservationUserProvisioningManager _reservationUserProvisioning;
    private static readonly object FaceProofSync = new();
    private const string FaceChallengeSessionKey = "FaceLoginChallenge";

    private sealed record VerifiedFaceProof(int KioskId, long EnrollId);



    public AuthenticationController(IAuthManager _AuthManager, IUserLogManager _UserLogManager,
        IConfiguration configuration,
        IRedisManager _Redis,
        IConstantManager constantManager,
        ILogger<AuthenticationController> logger,
        IUserManager userManager,
        IWebApiManager webApiManager,
        ApiTokenCacheClient apiTokenClient,
        IMemoryCache faceEventCache,
        IReservationUserProvisioningManager reservationUserProvisioning,
        IFajrLogManager fajrLogManager = null
        ) : base()
    {
        _configuration = configuration;
        _logger = logger;
        UserManager = userManager;
        WebApiManager = webApiManager;
        ApiTokenClient = apiTokenClient;
        _faceEventCache = faceEventCache;
        _reservationUserProvisioning = reservationUserProvisioning;
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
    [ValidateAntiForgeryToken]
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

        // هر ورود با رمز، مجوز استثنای ورود چهره قبلی را حذف می‌کند.
        HttpContext.Session.Remove("VerifiedFaceLogin");

        var res = AuthManager.Login(Mobile, Password);
        var User = res.Model as UserSessionDTO;

        // ورود با رمز، به هیچ عنوان حساب جدید از روی کد پرسنلی نمی‌سازد.
        // کد پرسنلی یک راز نیست و ثبت حساب بدون احراز هویت چهره می‌تواند
        // به تصاحب حساب فرد دیگری منجر شود.
        if (!res.Status && User == null)
        {
            res.Message =
                "حساب کاربری یا کلمه عبور معتبر نیست. برای ثبت اولیه حساب، از دستگاه تشخیص چهره استفاده کنید.";
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

        // ورود با نام کاربری نباید از مجوز نشست ورود چهره قبلی استفاده کند.
        HttpContext.Session.Remove("VerifiedFaceLogin");

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
        var configuredKioskId = _configuration.GetValue<int?>("ApiAddress:KioskId");

        if (mid == null)
        {
            var currentUser = HttpContext.Session.GetUser();
            if (currentUser != null)
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        // برای هر نمایش صفحه یک چالش تصادفیِ مستقل از کد پرسنلی ساخته
        // می‌شود. سرویس معتبر ZP باید همان چالش را در رویداد امضاشده
        // برگرداند؛ رویداد کاربر دیگر یا مرورگر دیگر پذیرفته نمی‌شود.
        var challenge = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        HttpContext.Session.SetString(FaceChallengeSessionKey, challenge);
        if (configuredKioskId.HasValue && configuredKioskId.Value > 0)
        {
            _faceEventCache.Set(
                $"FacePending:{challenge}",
                configuredKioskId.Value,
                TimeSpan.FromMinutes(3));
        }

        ViewBag.ApiBaseUrl = hubBaseUrl;
        ViewBag.KioskId = configuredKioskId;
        ViewBag.FaceChallenge = challenge;

        _logger.LogInformation(
            "صفحه انتظار چهره ایجاد شد. KioskId: {KioskId}",
            configuredKioskId);

        var showCaptcha = HttpContext.GetCookieShowCaptcha();
        if (showCaptcha)
            ViewBag.captcha = true;

        return View("IndexZP");
    }

    /// <summary>
    /// دریافت اعلان امضاشده از سرویس معتبر ZP.
    /// فقط سرویس دستگاه (نه JavaScript مرورگر) باید این endpoint را فراخوانی کند.
    /// Message-to-sign: KioskId:EnrollId:TimestampUtc:Nonce:SessionChallenge
    /// Signature: HMAC-SHA256 hex با FaceLogin:WebhookSecret
    /// </summary>
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult RecordVerifiedFace([FromBody] VerifiedFaceEvent payload)
    {
        var secret = _configuration["FaceLogin:WebhookSecret"];
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
        {
            _logger.LogError(
                "امضای رویداد چهره فعال نیست: Secret معتبر تنظیم نشده است.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        var expectedKiosk = _configuration.GetValue<int?>("ApiAddress:KioskId");
        if (!expectedKiosk.HasValue || expectedKiosk.Value <= 0 ||
            payload == null || payload.KioskId != expectedKiosk.Value ||
            payload.EnrollId <= 0 || payload.TimestampUtc <= 0 ||
            string.IsNullOrWhiteSpace(payload.Nonce) ||
            payload.Nonce.Length > 100 ||
            string.IsNullOrWhiteSpace(payload.SessionChallenge) ||
            payload.SessionChallenge.Length != 64 ||
            !payload.SessionChallenge.All(x =>
                (x >= '0' && x <= '9') || (x >= 'A' && x <= 'F')) ||
            string.IsNullOrWhiteSpace(payload.Signature))
        {
            return BadRequest();
        }

        DateTimeOffset timestamp;
        try
        {
            timestamp = DateTimeOffset.FromUnixTimeSeconds(payload.TimestampUtc);
        }
        catch (ArgumentOutOfRangeException)
        {
            return BadRequest();
        }

        if (Math.Abs((DateTimeOffset.UtcNow - timestamp).TotalSeconds) > 30)
            return Unauthorized();

        // امضای دستگاه لینوکسی و وب‌سرور ویندوزی باید مستقل
        // از فرهنگ/زبان پیش‌فرض سیستم دقیقاً روی یک رشته محاسبه شود.
        var signedMessage = FormattableString.Invariant(
            $"{payload.KioskId}:{payload.EnrollId}:{payload.TimestampUtc}:{payload.Nonce}:{payload.SessionChallenge}");

        try
        {
            var provided = Convert.FromHexString(payload.Signature.Trim());
            var expected = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(secret),
                Encoding.UTF8.GetBytes(signedMessage));

            if (!CryptographicOperations.FixedTimeEquals(provided, expected))
                return Unauthorized();
        }
        catch (FormatException)
        {
            return Unauthorized();
        }

        var challengeKey = $"FacePending:{payload.SessionChallenge}";
        var replayKey = $"VerifiedFaceNonce:{payload.KioskId}:{payload.Nonce}";
        var proofKey = $"FaceVerified:{payload.SessionChallenge}";

        lock (FaceProofSync)
        {
            // امضا کافی نیست: چالش باید از صفحه انتظار واقعی
            // صادر شده باشد، در همان کیوسک و هنوز معتبر باشد.
            if (!_faceEventCache.TryGetValue(challengeKey, out int pendingKiosk) ||
                pendingKiosk != payload.KioskId)
                return Conflict();

            if (_faceEventCache.TryGetValue(replayKey, out _) ||
                _faceEventCache.TryGetValue(proofKey, out _))
                return Conflict();

            _faceEventCache.Set(replayKey, true, TimeSpan.FromMinutes(3));
            _faceEventCache.Set(
                proofKey,
                new VerifiedFaceProof(payload.KioskId, payload.EnrollId),
                TimeSpan.FromSeconds(45));
        }

        _logger.LogInformation(
            "رویداد امضاشده و وابسته به نشست چهره تأیید شد. KioskId: {KioskId}, EnrollId: {EnrollId}",
            payload.KioskId, payload.EnrollId);

        return Ok(new { Status = true });
    }

    private bool ConsumeVerifiedFaceEvent(int kioskId, long enrollId)
    {
        var challenge = HttpContext.Session.GetString(FaceChallengeSessionKey);
        if (string.IsNullOrWhiteSpace(challenge))
            return false;

        var proofKey = $"FaceVerified:{challenge}";
        lock (FaceProofSync)
        {
            if (!_faceEventCache.TryGetValue(
                    proofKey, out VerifiedFaceProof proof) ||
                proof.KioskId != kioskId || proof.EnrollId != enrollId)
                return false;

            _faceEventCache.Remove(proofKey);
            _faceEventCache.Remove($"FacePending:{challenge}");
            HttpContext.Session.Remove(FaceChallengeSessionKey);
            return true;
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FinalizeFaceLogin(int kioskId, long enrollId)
    {
        _logger.LogInformation(
            "ورود بیومتریک: درخواست ورود دریافت شد. KioskId: {KioskId}، کد پرسنلی: {EnrollId}",
            kioskId,
            enrollId);

        if (kioskId <= 0 || enrollId <= 0)
        {
            _logger.LogWarning(
                "ورود بیومتریک: اطلاعات نامعتبر است. KioskId: {KioskId}، کد پرسنلی: {EnrollId}",
                kioskId,
                enrollId);

            TempData["FaceLoginMessage"] = "اطلاعات ورود معتبر نیست.";
            return RedirectToAction("IndexZP");
        }

        var configuredKiosk = _configuration.GetValue<int?>("ApiAddress:KioskId");
        if (configuredKiosk.HasValue && configuredKiosk.Value > 0 &&
            kioskId != configuredKiosk.Value)
        {
            _logger.LogWarning("درخواست ورود با شناسه کیوسک ناشناس. KioskId: {KioskId}", kioskId);
            TempData["FaceLoginMessage"] = "شناسه دستگاه ورود معتبر نیست.";
            return RedirectToAction("IndexZP");
        }

        // به هیچ عنوان امکان ورود بدون امضای معتبر و نشست انتظار وجود ندارد.
        // مقدار RequireSignedEvents=false نیز نباید این الزام را دور بزند.
        var verifiedFace = ConsumeVerifiedFaceEvent(kioskId, enrollId);
        if (!verifiedFace)
        {
            _logger.LogWarning(
                "ورود چهره بدون اثبات معتبر و وابسته به نشست رد شد. KioskId: {KioskId}, EnrollId: {EnrollId}",
                kioskId, enrollId);
            TempData["FaceLoginMessage"] =
                "تأیید تشخیص چهره برای این نشست دریافت نشده یا منقضی شده است. دوباره روبه‌روی دستگاه قرار بگیرید.";
            return RedirectToAction("IndexZP");
        }

        // بعضی سازمان‌ها کد پرسنلی را با صفرهای ابتدایی ذخیره می‌کنند.
        // طول کد فقط در صورت تنظیم صریح مشخص می‌شود؛ مقدار پیش‌فرض
        // همان کد واقعی فرستاده‌شده از دستگاه است.
        var personCode = enrollId.ToString();
        var configuredCodeLength =
            _configuration.GetValue<int>("FaceLogin:PersonnelCodeLength");
        if (configuredCodeLength > 0 && configuredCodeLength <= 30)
            personCode = personCode.PadLeft(configuredCodeLength, '0');

        var user = AuthManager.LoginWithFaceZP(personCode);

        if (user == null && verifiedFace)
        {
            try
            {
                // صرفاً شناسایی معتبر و امضاشده دستگاه اجازه ساخت حساب جدید می‌دهد.
                var accessToken = await ApiTokenClient.GetApiToken(
                    CustomSettings.Instance.ClientId,
                    CustomSettings.Instance.Scope,
                    CustomSettings.Instance.ClientSecret,
                    CustomSettings.Instance.ROPC_UserName,
                    CustomSettings.Instance.ROPC_Password);

                var person = WebApiManager.GetPersonalByPersonCode(
                    personCode, accessToken);

                if (person == null ||
                    person.personalCode?.Trim().ToEnglishNumber() != personCode ||
                    person.Id <= 0)
                {
                    TempData["FaceLoginMessage"] =
                        "کد پرسنلی شناسایی‌شده در سرویس معتبر پرسنلی یافت نشد.";
                    return RedirectToAction("IndexZP");
                }

                var fullName = !string.IsNullOrWhiteSpace(person.FullName)
                    ? person.FullName
                    : $"{person.FirstName} {person.LastName}".Trim();

                var result = _reservationUserProvisioning.EnsureReservationUser(
                    person.Id,
                    personCode,
                    fullName,
                    person.MelliCode,
                    null,
                    0, // ایجاد خودکار توسط سرویس، نه یک کاربر پنل
                    person.UnitCode);

                if (!result.Status)
                {
                    _logger.LogWarning(
                        "ایجاد حساب هنگام ورود چهره موفق نبود. EnrollId: {EnrollId}, Reason: {Reason}",
                        enrollId, result.Message);
                    TempData["FaceLoginMessage"] = result.Message;
                    return RedirectToAction("IndexZP");
                }

                user = AuthManager.LoginWithVerifiedFacePersonnel(
                    person.Id, personCode);
                if (user != null)
                {
                    _logger.LogInformation(
                        "حساب معتبر پرسنلی پس از تأیید ZP برای ورود آماده شد. UserId: {UserId}, EnrollId: {EnrollId}",
                        user.Id, enrollId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "استعلام و ساخت حساب در اولین ورود با چهره ناموفق بود. EnrollId: {EnrollId}",
                    enrollId);
                TempData["FaceLoginMessage"] =
                    "در حال حاضر ایجاد حساب از طریق سرویس پرسنلی امکان‌پذیر نیست.";
                return RedirectToAction("IndexZP");
            }
        }

        if (user == null || !user.IsEnabled)
        {
            _logger.LogWarning(
                "ورود بیومتریک: حساب معتبر یافت نشد. KioskId: {KioskId}, EnrollId: {EnrollId}",
                kioskId, enrollId);
            TempData["FaceLoginMessage"] =
                verifiedFace
                    ? "حساب پرسنلی یافت نشد یا غیرفعال است."
                    : "برای ثبت خودکار حساب، تأیید امن سرویس تشخیص چهره لازم است.";
            return RedirectToAction("IndexZP");
        }

        _logger.LogInformation(
            "ورود بیومتریک: کاربر پیدا شد. KioskId: {KioskId}، کد پرسنلی: {EnrollId}، UserId: {UserId}",
            kioskId,
            enrollId,
            user.Id);

        try
        {
            var token = await Redis.db.SetLoginToken(user.Id);
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("توکن نشست کاربر از سرویس ورود دریافت نشد.");

            HttpContext.SetCookieUserToken(token);
            HttpContext.Session.SetUser(user);
            HttpContext.Session.SetString("VerifiedFaceLogin", "true");
        }
        catch (Exception ex)
        {
            // AuthManager ممکن است نشست را قبل از SetLoginToken ساخته باشد.
            // خطای Redis نباید اجازه دهد نشست نیمه‌کاره به‌عنوان ورود موفق بماند.
            HttpContext.Session.RemoveUser();
            HttpContext.Session.Remove("VerifiedFaceLogin");

            _logger.LogError(ex,
                "تکمیل نشست ورود چهره ناموفق بود. UserId: {UserId}", user.Id);
            TempData["FaceLoginMessage"] =
                "ورود چهره تأیید شد، اما ایجاد نشست کاربری انجام نشد. دوباره تلاش کنید.";
            return RedirectToAction("IndexZP");
        }

        _logger.LogInformation(
            "ورود بیومتریک: موفق؛ انتقال به داشبورد. UserId: {UserId}",
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
            Session.Remove("VerifiedFaceLogin");

            // پاک کردن کوکی
            HttpContext.Response.Cookies.Delete("_session.cookie"); // توجه: اسم کوکی باید دقیق باشه
        }

        return RedirectToAction("IndexZP", "Authentication");
    }
    #endregion





}
/// <summary>
/// اعلان امضاشده ZP برای اثبات رویداد تشخیص چهره.
/// این مدل نباید مستقیماً از مرورگر ارسال شود.
/// </summary>
public sealed class VerifiedFaceEvent
{
    public int KioskId { get; set; }
    public long EnrollId { get; set; }
    public long TimestampUtc { get; set; }
    public string Nonce { get; set; }
    public string SessionChallenge { get; set; }
    public string Signature { get; set; }
}
