using BLL.Interface;
using DTO.Menu;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.SessionServices;

namespace LearningSkill.Areas.Global.Controllers
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