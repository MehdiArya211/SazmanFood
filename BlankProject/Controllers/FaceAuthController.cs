using BLL.Interface;
using DTO.User;
using Microsoft.AspNetCore.Mvc;
using Services.CookieServices;
using Services.RedisService;

public class FaceAuthController : Controller
{
    private readonly IUserManager _userManager;
    private readonly IAuthManager AuthManager;
    private readonly IRedisManager _redis;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ISession Session;
    private readonly IConstantManager _constantManager;


    public FaceAuthController(IUserManager userManager, IRedisManager redis,
        IConstantManager constantManager, IHttpContextAccessor httpContextAccessor, IAuthManager _authManager)
    {
        _userManager = userManager;
        _constantManager = constantManager;
        _redis = redis;
        this.httpContextAccessor = httpContextAccessor;
        Session = httpContextAccessor.HttpContext.Session;
        AuthManager = _authManager;
    }



    [HttpGet]
    public async Task<IActionResult> LoginByFace(string userId, string correlationId)
    {

        var user = _userManager.GetAll().Where(x => x.Username == userId).FirstOrDefault();

        #region بررسی تعداد تلاش برای لاگین و قفل بودن حساب کاربری
        //لاگ دفعات تلاش برای لاگین
        var loginLog = await _redis.db.SetLoginLog(user.Username);
        var FailedLoginCount = _constantManager.GetFailedLoginCount();
        //if (loginLog != null && loginLog.Count > FailedLoginCount)
        //{

        //    return View();
        //}
        #endregion

        #region عملیات لاگین
        var res = AuthManager.LoginAuth(userId);
        var User = res.Model as UserSessionDTO;
        if (!res.Status)
        {
            //await _redis.db.SetLoginLog(_redis.ContextAccessor, "e", user.Username, User?.FullName, User?.Id, false, res.Message);
            if (User == null)
            {
                ViewBag.Error = res.Message;
                ViewBag.InvalidLogin = true;

                return View();
            }
        }
        #endregion

        #region اطلاعات ردیس و کوکی

 
            // افزودن توکن کاربر به ردیس برای جلوگیری از لاگین همزمان 2 نفر با یک اکانت
            var token = await _redis.db.SetLoginToken(user.Id);

            // افزودن توکن به کوکی
            HttpContext.SetCookieUserToken(token);
        // 🟢 تمدید TTL سشن بعد از لاگین موفق
        await _redis.db.RefreshUserSession(user.Id, /*DeviceId*/ 1, 3);//TODO change device id



        // حذف اطلاعات مربوط به کنترل تعداد دفعات تلاش برای لاگین
        //` await _redis.db.RemoveLoginLog(user.Username);
        #endregion


        return RedirectToAction("index", "Dashboard", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> LoginByFace1(string userId, string correlationId)
    {
        try
        {
            var MenuId = 1;
            var user = _userManager.GetAll().Where(x => x.Username == userId).FirstOrDefault();

            if (user == null)
                return RedirectToAction("Login", "Account", new { area = "Admin" });


            var res = AuthManager.LoginAuth(userId);

            var User = res.Model as UserSessionDTO;


            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            //return Redirect("~/admin/Dashboard/index1");
            // return Redirect("http://porsan.net/");
        }
        catch (Exception ex)
        {
            // در صورت نیاز لاگ خطا هم می‌توانید اضافه کنید
            return StatusCode(500, "خطا در پردازش");
        }
    }


}
