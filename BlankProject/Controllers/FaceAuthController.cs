using BLL.Interface;
using DTO.User;
using Microsoft.AspNetCore.Mvc;
using Services.CookieServices;
using Services.RedisService;

namespace Food.Controllers
{
    /// <summary>
    /// ورود کاربران از طریق تشخیص چهره
    /// </summary>
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



    // این دو مسیر قدیمی با UserId/Username دریافتی از URL ورود
    // بدون اثبات چهره ایجاد می‌کردند؛ صرفاً به مسیر امن ZP هدایت می‌شوند.
    [HttpGet]
    public IActionResult LoginByFace(string userId, string correlationId)
    {
        return RedirectToAction("IndexZP", "Authentication", new { area = "" });
    }

    [HttpGet]
    public IActionResult LoginByFace1(string userId, string correlationId)
    {
        return RedirectToAction("IndexZP", "Authentication", new { area = "" });
    }

}
}
