using BLL;
using BLL.Interface;
using Domain.Enums;
using DTO.Base;
using DTO.User;
using FajrLog.Enum;
using Filters;
using ITOWebApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.RedisService;
using Services.SessionServices;
using Utilities.Extentions;

namespace Food.Areas.AuthSystem.Controllers
{
    /// <summary>
    /// مدیریت کاربران - پرسنل
    /// </summary>
    [Area("AuthSystem")]
    [UserAuthorize(Area: "AuthSystem", Controller: "users", Action: "index")]
    public class UsersController : Controller
    {
        private readonly IUserManager UserManager;
        private readonly IRoleManager roleManager;
        private readonly IUserPasswordHistoryManager PasswordHistoryManager;
        private readonly IDataTableManager dataTableManager;
        private readonly IRedisManager Redis;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IWebApiManager webApiManager;
        private readonly ISession Session;

        private string access_token = string.Empty;

        public UsersController(
            IRedisManager _Redis,
            IHttpContextAccessor _httpContextAccessor,
            IUserManager _UserManager,
            IRoleManager _roleManager,
            IUserPasswordHistoryManager _PasswordHistoryManager,
            IDataTableManager _dataTableManager,
            ApiTokenCacheClient apiTokenCache,
            IWebApiManager _webApiManager)
        {
            UserManager = _UserManager;
            roleManager = _roleManager;
            dataTableManager = _dataTableManager;
            Redis = _Redis;
            _apiTokenClient = apiTokenCache;
            webApiManager = _webApiManager;
            Session = _httpContextAccessor.HttpContext.Session;

            access_token = _apiTokenClient.GetApiToken(
                CustomSettings.Instance.ClientId,
                CustomSettings.Instance.Scope,
                CustomSettings.Instance.ClientSecret,
                CustomSettings.Instance.ROPC_UserName,
                CustomSettings.Instance.ROPC_Password
            ).Result;

            PasswordHistoryManager = _PasswordHistoryManager;
        }

        #region نمایش همه

        public IActionResult Index()
        {
            ViewData["Roles"] = new SelectList(
                roleManager.GetSelectListDTO(),
                "Id",
                "Title"
            );

            return View();
        }

        [HttpPost]
        public ActionResult GetList(UserFilterDataTableDTO filters)
        {
            var SearchModel = dataTableManager.GetSearchModel();

            var model = UserManager.GetDataTableDTO(
                SearchModel,
                filters
            );

            return Json(model);
        }

        #endregion

        #region ایجاد

        public IActionResult LoadCreateForm()
        {
            ViewData["RoleId"] = new SelectList(
                roleManager.GetSelectListDTO(),
                "Id",
                "Title"
            );

            ViewData["UserType"] = new SelectList(
                EnumExtensions.ToEnumViewModel<UserType>(),
                "Id",
                "Title"
            );

            ViewData["Gharargah"] = new SelectList(
                webApiManager.GetOrganInfo(access_token),
                "Id",
                "UnitTitle"
            );


            return PartialView("_Create", new UserCreateDTO());
        }


        //[HttpGet]
        //public IActionResult GetOrgByGharargah(int id)
        //{
        //    var result = webApiManager.GetOrganInfo(
        //        id,
        //        access_token
        //    );

        //    return Json(result);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserCreateDTO model)
        {
            try
            {
                if (model.RoleId == 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "نوع دسترسی کاربر را مشخص کنید!"
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    return Json(new
                    {
                        Status = false,
                        Message = string.Join("</br>", errors)
                    });
                }

                var res = UserManager.Create(model);

                PasswordHistoryManager.CheckPasswordHistory(
                    res.Model,
                    model.Password,
                    false
                );

                return Json(res);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ثبت اطلاعات با خطا همراه بوده است!"
                });
            }
        }


        #endregion


        #region ویرایش

        public IActionResult LoadEditForm(long id)
        {
            var user = UserManager.GetEditDTO(id);

            if (user == null)
                return NotFound();

            ViewData["RoleId"] = new SelectList(
                roleManager.GetSelectListDTO(),
                "Id",
                "Title",
                user.RoleId
            );

            ViewData["UserType"] = new SelectList(
                EnumExtensions.ToEnumViewModel<UserType>(),
                "Id",
                "Title",
                user.Type
            );

            ViewData["Gharargah"] = new SelectList(
                webApiManager.GetOrganInfo(access_token),
                "Id",
                "UnitTitle",
                user.OmdOrgId
            );


            return PartialView("_Edit", user);
        }


        [HttpGet]
        public IActionResult GetEditOrgByGharargah(int id)
        {
            var result = webApiManager.GetOrganByGharargahId(
                id,
                access_token
            );

            return Json(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, UserEditDTO model)
        {
            try
            {
                if (id != model.Id)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "کاربر یافت نشد!"
                    });
                }

                if (model.RoleId == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "نوع دسترسی کاربر را مشخص کنید!"
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    return Json(new
                    {
                        Status = false,
                        Message = string.Join("</br>", errors)
                    });
                }

                return Json(UserManager.Update(model));
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ثبت اطلاعات با خطا همراه بوده است!"
                });
            }
        }

        #endregion


        #region تغییر رمز

        public IActionResult LoadChangePasswordForm(long id, string FullName)
        {
            ViewBag.FullName = FullName;

            return PartialView(
                "_ChangePassword",
                new UserChangePasswordDTO
                {
                    Id = id
                }
            );
        }


        [HttpPost]
        public IActionResult ChangePassword(long id, UserChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Status = false,
                    Message = "اطلاعات صحیح نیست"
                });
            }

            return Json(
                UserManager.ChangePassword(model)
            );
        }

        #endregion


        #region فعال غیر فعال

        public IActionResult ToggleEnable(long id)
        {
            var model = UserManager.GetById(id);

            model.IsEnabled = !model.IsEnabled;

            var res = UserManager.Update(model);

            return Json(new
            {
                res.Status,
                model.IsEnabled
            });
        }

        #endregion


        #region حذف

        [HttpPost]
        public IActionResult Delete(long id)
        {
            var result = UserManager.Delete(id);

            return Json(new
            {
                Status = result,
                Message = result
                    ? "کاربر حذف شد"
                    : "حذف انجام نشد"
            });
        }

        #endregion
    }
}