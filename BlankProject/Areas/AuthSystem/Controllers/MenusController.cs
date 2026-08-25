using BLL.Interface;
using Domain.Entities;
using Domain.Enums;
using FajrLog.Enum;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.RedisService;

namespace Food.Areas.AuthSystem.Controllers
{

    /// <summary>
    /// مدیریت منو های مشتریان
    /// </summary>
    [Area("AuthSystem")]
    [UserAuthorize(Area: "AuthSystem", Controller: "menus", Action: "index")]
    public class MenusController : Controller
    {
        private readonly IMenuManager menuManager;
        private readonly IRedisManager Redis;
        public MenusController(IRedisManager _Redis, IMenuManager _menuManager)
        {
            menuManager = _menuManager;
            Redis = _Redis;
        }

        #region نمایش همه
        public IActionResult Index()
        {
            return View();
        }


        #region گرفتن همه منو ها با اجکس
        [HttpGet]
        public IActionResult GetAll()
        {
            var model = menuManager.GetTreeViewDTO();
            return Json(model);
        }
        #endregion
        #endregion        


        #region ایجاد
        /// <summary>
        /// لود کردن فرم ایجاد منو در مدال
        /// </summary>
        /// <param name="ParentId">شناسه والد</param>
        /// <param name="ParentName">عنوان والد</param>
        /// <returns></returns>
        public IActionResult LoadCreateForm(long? ParentId, string ParentName)
        {
            var model = new Menu
            {
                ParentId = ParentId,
            };

            ViewBag.ParentName = ParentName;
            return PartialView("_Create", model);
        }


        /// <summary>
        /// ایجاد منو جدید
        /// </summary>
        /// <param name="model">منو</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Menu model)
        {
            FajrActionType? fajrActionType = model.NeedReAuthorize ? FajrActionType.defineSecurePage : null;
            if (ModelState.IsValid)
            {
                var res = menuManager.Create(model);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Create, MenuType.Menus, res.Status,
                    res.Status ? $"منو {model.Title} با آیدی {(long?)res.Model} : " + res.Message : res.Message,
                    res.Status ? (long?)res.Model : null, fajrActionType).Result;
                return Json(new { res.Status, res.Message });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Create, MenuType.Menus, false, string.Join("/", errors), null, fajrActionType).Result;
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
        /// لود کردن فرم ویرایش منو در مدال
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadEditForm(long? id)
        {
            var model = menuManager.GetById(id);
            return PartialView("_Edit", model);
        }


        /// <summary>
        /// ویرایش اطلاعات منو
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Menu model)
        {
            if (ModelState.IsValid)
            {
                var res = menuManager.Update(model);
                #region برای مشخص شدن عملیات لاگ فجر
                var oldNeed = (res.Model as bool?) ?? false;
                FajrActionType? fajrActionType = null;
                if (!oldNeed && model.NeedReAuthorize)
                    fajrActionType = FajrActionType.defineSecurePage;
                else if (oldNeed && !model.NeedReAuthorize)
                    fajrActionType = FajrActionType.removeSecurePage; 
                #endregion
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Update, MenuType.Menus, res.Status, $"منو {model.Title} با آیدی {model.Id} : " + res.Message, model.Id, fajrActionType).Result;
                return Json(new { res.Status, res.Message });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Update, MenuType.Menus, false, $"منو {model.Title} با آیدی {model.Id} : " + string.Join("/", errors)).Result;
                return Json(new
                {
                    Status = false,
                    Message = string.Join("</br>", errors)
                });
            }
        }

        #endregion


        #region حذف
        /// <summary>
        /// حذف منو
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(long id)
        {
            var HasChild = menuManager.HasChild(id);
            if (HasChild)
            {
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Remove, MenuType.Menus, false, $"منو با آیدی {id} : " + "لطفا ابتدا زیر مجموعه های این منو را حذف کنید!").Result;
                return Json(new { Status = false, Message = "لطفا ابتدا زیر مجموعه های این منو را حذف کنید!" });
            }
            var res = menuManager.DeleteWithRoles(id);
            FajrActionType? fajrActionType = ((res.Model as bool?) ?? false ) ? FajrActionType.removeSecurePage : null;
            _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Remove, MenuType.Menus, res.Status, $"منو با آیدی {id} : " + res.Message, id, fajrActionType).Result;
            return Json(res);
        }
        #endregion


    }
}
