using BLL;
using BLL.Interface;
using Domain.Enums;
using DTO.User;
using DTO.UserLog;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.RedisService;
using Services.SessionServices;
using FajrLog.Enum;
using DTO.Base;

namespace Food.Areas.Admin.Controllers
{

    /// <summary>
    /// پروفایل کاربر
    /// </summary>

    [Area("Admin")]
    //[UserAuthorize(Area: "admin", Controller: "profile", Action: "edit")]
    [UserAuthorize(IsPublic: true, CheckPasswordChange: false)]
    public class ProfileController : Controller
    {
        private readonly IUserManager userManager;
        private readonly IUserPasswordHistoryManager PasswordHistoryManager;

        private readonly ISession session;
        private readonly IRedisManager Redis;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserLogManager logManager;
        private readonly IDataTableManager dataTableManager;

        public ProfileController(IUserManager _userManager, IHttpContextAccessor _httpContextAccessor, IUserPasswordHistoryManager _PasswordHistoryManager, IRedisManager _Redis,
                                 IUserLogManager _logManager, IDataTableManager _dataTableManager)
        {
            logManager = _logManager;
            dataTableManager = _dataTableManager;
            userManager = _userManager;
            PasswordHistoryManager = _PasswordHistoryManager;
            httpContextAccessor = _httpContextAccessor;
            Redis = _Redis;
            session = _httpContextAccessor.HttpContext.Session;
        }


        #region نمایش ورود و خروج
        public IActionResult LoginLog()
        {
            return View();
        }


        /// <summary>
        /// لیست داده مورد نیاز برای دیتاتیبل
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLoginLog(UserLoginLogFilter filters)
        {
            var user = session.GetUser();
            filters.UserId = user.Id;
            var SearchModel = dataTableManager.GetSearchModel();
            var model = logManager.GetLoginDataTableDTO(SearchModel, filters);
            return Json(model);
        }

        #endregion


        #region ویرایش
        [UserAuthorize(IsPublic: true)]
        public IActionResult Edit()
        {
            var user = session.GetUser();
            var model = userManager.GetEditProfileDTO(user.Id);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserEditProfileDTO model)
        {
            if (ModelState.IsValid)
            {
                var res = userManager.UpdateProfile(model);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Update, MenuType.Profile, res.Status, $"کاربر {model.Username} با آیدی {model.Id} : " + res.Message, model.Id, FajrActionType.editProfile).Result;
                return Json(res);
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Update, MenuType.Profile, false, $"کاربر {model.Username} با آیدی {model.Id} : " + string.Join("/", errors), model.Id, FajrActionType.editProfile).Result;
                return Json(new
                {
                    Status = false,
                    Message = string.Join("</br>", errors)
                });
            }
        }

        #endregion


        #region تغییر کلمه عبور
        public IActionResult ChangePassword()
        {
            var user = session.GetUser();
            ViewBag.FullName = user.FullName;
            var model = new UserProfileChangePasswordDTO() { Id = user.Id, PasswordIsChanged = user.PasswordIsChanged };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(UserProfileChangePasswordDTO model, string Captcha)
        {
            var captcha = HttpContext.Session.GetString("Captcha");
            if (string.IsNullOrEmpty(captcha) || captcha != Captcha)
            {
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.ChangePassword, MenuType.Profile, false, $"کاربر با آیدی {model.Id} : " + "کد امنیتی صحیح نیست!", model.Id, FajrActionType.changePassword).Result;
                return Json(new { Status = false, Message = "کد امنیتی صحیح نیست!" });
            }

            if (ModelState.IsValid)
            {
                // چک میشود که پسورد ورودی برابر سه تا پسورد اخر نباشد
                bool CheckRepitLatThreePassword = PasswordHistoryManager.CheckPasswordHistory(model.Id, model.Password, true);
                var res = new BaseResult();

                if (CheckRepitLatThreePassword == false)
                {
                    res = userManager.ProfileChangePassword(model);
                }
                else
                {
                    return Json(new
                    {
                        Status = false,
                        Message = string.Join("</br>", " پسورد شما نباید با سه تا پسورد آخر برابر باشد لطفا پسورد جدیدی وارد کنید")
                    });
                }
                 
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.ChangePassword, MenuType.Profile, res.Status, $"کاربر با آیدی {model.Id} : " + res.Message, model.Id, FajrActionType.changePassword).Result;
                return Json(res);
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Update, MenuType.Profile, false, $"کاربر با آیدی {model.Id} : " + string.Join("/", errors), model.Id, FajrActionType.changePassword).Result;
                return Json(new
                {
                    Status = false,
                    Message = string.Join("</br>", errors)
                });
            }
        }
        #endregion

    }
}
