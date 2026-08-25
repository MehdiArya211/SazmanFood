using BLL.Interface;
using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using FajrLog.Enum;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.RedisService;
using Utilities.Extentions;

namespace Food.Areas.AuthSystem.Controllers
{

    /// <summary>
    /// مدیریت نقش های کاربران
    /// </summary>
    [Area("AuthSystem")]
    [UserAuthorize(Area: "AuthSystem", Controller: "roles", Action: "index")]
    public class RolesController : Controller
    {
        private readonly IRoleManager roleManager;
        private readonly IRoleMenuManager roleMenuManager;
        private readonly IMenuManager menuManager;
        private readonly IDataTableManager dataTableManager;
        private readonly IRedisManager Redis;
        public RolesController(IRedisManager _Redis, IRoleManager _roleManager, IMenuManager _menuManager, IRoleMenuManager _roleMenuManager, IDataTableManager _dataTableManager)
        {
            roleManager = _roleManager;
            menuManager = _menuManager;
            roleMenuManager = _roleMenuManager;
            dataTableManager = _dataTableManager;
            Redis = _Redis;
        }


        #region نمایش همه
        public IActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// لیست داده مورد نیاز برای دیتاتیبل
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetList()
        {
            var SearchModel = dataTableManager.GetSearchModel();
            var model = roleManager.GetDataTableDTO(SearchModel);
            return Json(model);
        }

        #endregion


        #region ایجاد

        /// <summary>
        /// لود کردن فرم ایجاد نقش در مدال
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadCreateForm()
        {
            ViewData["RoleType"] = new SelectList(EnumExtensions.ToEnumViewModel<UserType>(), "Id", "Title");
            var model = new Role();
            return PartialView("_Create", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role model)
        {
            if (ModelState.IsValid)
            {
                var res = await roleManager.CreateAsync(model);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Create, MenuType.Roles, res.Status,
                    res.Status ? $"نقش {model.Title} با آیدی {(long?)res.Model} : " + res.Message : res.Message,
                    res.Status ? (long?)res.Model : null, FajrActionType.creatRole).Result;
                return Json(res);
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Create, MenuType.Roles, false, string.Join("/", errors),null, FajrActionType.creatRole).Result;
                return Json(new
                {
                    Status = false,
                    Message = string.Join("</br>", errors)
                });
            }
        }
        #endregion


        #region ویرایش
        /// <summary>
        /// لود کردن فرم ایجاد نقش در مدال
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadEditForm(long id)
        {
            var role = roleManager.GetById(id);
            ViewData["RoleType"] = new SelectList(EnumExtensions.ToEnumViewModel<UserType>(), "Id", "Title");
            return PartialView("_Edit", role);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Role model)
        {
            if (ModelState.IsValid)
            {
                var res = roleManager.Update(model);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Update, MenuType.Roles, res.Status, $"نقش {model.Title} با آیدی {model.Id} : " + res.Message, model.Id, FajrActionType.editRole).Result;
                return Json(res);
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Update, MenuType.Roles, false, $"نقش {model.Title} با آیدی {model.Id} : " + string.Join("/", errors), model.Id, FajrActionType.editRole).Result;
                return Json(new
                {
                    Status = false,
                    Message = string.Join("</br>", errors)
                });
            }
        }

        #endregion


        #region منوهایی که نقش به آنها دسترسی دارد 

        #region گرفتن لیست منو ها به همراه دسترسی های یک نقش
        // لیست همه دسترسی به منوها
        public IActionResult LoadAccess(int id)
        {
            var RoleMenus = roleMenuManager.GetByRoleId(id);
            return Json(RoleMenus);
        }


        // لیست همه منو های فعال
        public IActionResult LoadMenus()
        {
            var Menus = menuManager.GetTreeViewDTO(true);
            return Json(Menus);
        }
        #endregion


        #region ذخیره دسترسی های نقش
        [HttpPost]
        public IActionResult SaveAccess(long RoleId, List<long> MenuIds)
        {
            if (MenuIds.Count == 0)
            {
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.AccessRoleMenu, MenuType.Roles, false, $"نقش با آیدی {RoleId} : " + "منویی انتخاب نشده است!", RoleId, FajrActionType.addRolePermission).Result;
                return Json(new BaseResult(false, "منویی انتخاب نشده است!"));
            }
            var res = roleMenuManager.UpdateRoleMenus(RoleId, MenuIds);
            _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.AccessRoleMenu, MenuType.Roles, res.Status, $"نقش با آیدی {RoleId} : " + res.Message, RoleId, FajrActionType.addRolePermission).Result;
            return Json(res);
        }
        #endregion

        #endregion



        #region تغییر وضعیت فعال بودن یا نبودن نقش 
        public IActionResult ToggleEnable(long id)
        {
            if (id == 0)
                return Json(new { Status = false });
            var model = roleManager.GetById(id);
            model.IsEnabled = !model.IsEnabled;
            var res = roleManager.Update(model);
            _ = Redis.db.SetLog(Redis.ContextAccessor, (model.IsEnabled ? ActionType.Enable : ActionType.Disable), MenuType.Roles, res.Status, $"نقش {model.Title} با آیدی {id} : " + res.Message, id).Result;
            return Json(new { res.Status, model.IsEnabled });
        }
        #endregion


        #region حذف
        [HttpPost]
        public IActionResult Delete(long id)
        {
            var res = roleManager.DeleteWithMenus(id);
            _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Remove, MenuType.Roles, res.Status, $"نقش با آیدی {id} : " + res.Message, id, FajrActionType.deleteRole).Result;
            return Json(res);
        }
        #endregion

    }
}
