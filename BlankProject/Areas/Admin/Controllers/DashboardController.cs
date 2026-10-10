using BLL;
using BLL.Interface;
using DTO.Diagrams;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Services.CacheServices;
using Services.SessionServices;

namespace Food.Areas.Admin.Controllers
{
    [Area("Admin")]
    [UserAuthorize(Area: "admin", Controller: "dashboard", Action: "index")]
    //[UserAuthorize(IsPublic: true)]
    public class DashboardController : Controller
    {
        private readonly IMemoryCache cache;
        private readonly IUserManager userManager;
        private readonly IUserLogManager UserLogManager;
        private readonly ISession session;


        public DashboardController(
            IUserManager _userManager,
            IUserLogManager userLogManager,
            IHttpContextAccessor _httpContextAccessor,
            IMemoryCache _cache)
        {
            userManager = _userManager;
            cache = _cache;
            session = _httpContextAccessor.HttpContext.Session;
            UserLogManager = userLogManager;
        }

        public IActionResult Index()
        {
            var User =
                HttpContext.Session.GetUser();

            TempData.Remove("QuotaMessage");

            if (TempData["Error"] is string error &&
                error.Contains("سهمیه‌ای برای شما ثبت نشده"))
            {
                TempData.Remove("Error");
            }

            return View(User);
        }

        #region پاک کردن کش

        public ActionResult ClearCache()
        {
            cache.ClearCache();
            return RedirectToAction("index");
        }

        #endregion

    }
}