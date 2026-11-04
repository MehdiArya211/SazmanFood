using BLL.Interface;
using Domain.Constants;
using DTO.Menu;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.SessionServices;

namespace Food.Areas.Global.Controllers
{
    [UserAuthorize(IsPublic: true, CheckPasswordChange: false)]
    [Area("Global")]
    public class ClientMenusController : Controller
    {
        private readonly ISession session;
        private readonly IUserManager userManager;
        private readonly IRoleMenuManager roleMenuManager;
        private readonly IAuthManager authManager;

        public ClientMenusController(IHttpContextAccessor _httpContextAccessor, IUserManager _userManager, IRoleMenuManager _roleMenuManager, IAuthManager _authManager)
        {
            userManager = _userManager;
            authManager = _authManager;
            roleMenuManager = _roleMenuManager;
            session = _httpContextAccessor.HttpContext.Session;
        }


        //[ResponseCache(VaryByHeader = "User-Agent" , Duration = 60)]
        public IActionResult LoadMenu(int UserId, string CController, string CAction)
        {
            try
            {
                var User = session.GetUser();
                List<MenuSessionDTO> Menus = new List<MenuSessionDTO>();
                if (User?.Menus != null)
                    Menus = authManager.ReshapeMenuData(User.Menus);
                ViewBag.Controller = CController;
                ViewBag.Action = CAction;
                ViewBag.CanControlQuota = User?.RoleId == RoleConstant.Admin && User.IsEnabled;

                // منوهای رزرو و چاپ برای همه نقش‌های واردشده نمایش داده شوند.
                // اگر قبلاً در نقش تعریف شده‌اند، لینک تکراری اضافه نکنیم.
                var assignedMenus = User?.Menus ?? new List<MenuSessionDTO>();
                ViewBag.AddReserveShortcut = !assignedMenus.Any(x =>
                    x.ShowInMenu &&
                    string.Equals(x.Area, "reservemanagment", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.Controller, "reserve", StringComparison.OrdinalIgnoreCase));
                ViewBag.AddPrintShortcut = !assignedMenus.Any(x =>
                    x.ShowInMenu &&
                    string.Equals(x.Area, "showreservemanagment", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.Controller, "showreserve", StringComparison.OrdinalIgnoreCase));

                ViewData["IsRootMenu"] = true;
                return PartialView("_Menus", Menus);
            }
            catch
            {
                List<MenuSessionDTO> Menus = new List<MenuSessionDTO>();
                return PartialView("_Menus", Menus);
            }
        }
    }
}