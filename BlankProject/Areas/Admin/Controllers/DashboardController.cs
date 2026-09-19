using BLL;
using BLL.Interface;
using DTO.Diagrams;
using Domain.Constants;
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
        private readonly IQoutaPersonManager qoutaPersonManager;
        private readonly ISession session;


        public DashboardController(IUserManager _userManager, IUserLogManager userLogManager,
            IHttpContextAccessor _httpContextAccessor, IMemoryCache _cache,
            IQoutaPersonManager _qoutaPersonManager
           )
        {
            userManager = _userManager;
            cache = _cache;
            session = _httpContextAccessor.HttpContext.Session;
            UserLogManager = userLogManager;
            qoutaPersonManager = _qoutaPersonManager;

        }

        public IActionResult Index()
        {
            var User = HttpContext.Session.GetUser();

            if (User?.RoleId == RoleConstant.MealBooker)
            {
                var personalCode = User.PersonCode?.ToString() ?? User.Username;

                if (!qoutaPersonManager.HasActiveQuota(personalCode))
                {
                    ViewBag.QuotaMessage = "سهمیه‌ای برای شما ثبت نشده است.";
                }
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