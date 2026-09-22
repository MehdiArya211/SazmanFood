//using BLL.Interface;
//using Domain.Enums;
//using DTO.Menu;
//using DTO.User;
//using FajrLog.Enum;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.DependencyInjection;
//using Services.CookieServices;
//using Services.RedisService;
//using Services.SessionServices;

//namespace Filters
//{
//    /// <summary>
//    /// بررسی وجود کاربر درون سشن به منظور لاگین
//    /// </summary>

//    public class UserAuthorize : ActionFilterAttribute
//    {
//        private bool isPublic;
//        private bool checkPasswordChange;
//        private string area;
//        private string controller;
//        private string action;

//        /// <summary>
//        /// بررسی دسترسی کاربر
//        /// </summary>
//        /// <param name="IsPublic">آیا ادرس مورد نظر عمومی است و همه به آن دسترسی دارند؟</param>
//        public UserAuthorize(bool IsPublic = false, string? Area = null, string? Controller = null, string? Action = null, bool CheckPasswordChange = true)
//        {
//            isPublic = IsPublic;
//            checkPasswordChange = CheckPasswordChange;
//            area = Area?.ToLower();
//            controller = Controller?.ToLower();
//            action = Action?.ToLower();
//        }



//        public override void OnActionExecuted(ActionExecutedContext context)
//        {
//        }


//        public override async void OnActionExecuting(ActionExecutingContext context)
//        {
//            var url = context.HttpContext.Request.Path.ToString()
//                        + context.HttpContext.Request.QueryString;


//            var isLoginWithFace = false;

//            #region اگر ادمین درون سشن نباشد کاربر به صفحه لاگین ریدایرکت میشود
//            var controllerObj = context.Controller as Controller;
//            var Redis = controllerObj?.HttpContext.RequestServices.GetService<IRedisManager>();
//            var User = controllerObj?.HttpContext.Session.GetUser();
//            if (User == null)
//            {
//                var authManager = controllerObj?.HttpContext.RequestServices.GetService<IAuthManager>();
//                var userDevice = await Redis.db.GetRedisUserDeviceData(1); // TODO: FOR TEST
//                if(userDevice != null)
//                {
//                    User = authManager.LoginWithFace(userDevice);

//                    if(User != null)
//                    {
//                        isLoginWithFace = true;
//                        var tk = await Redis.db.GetLoginToken(User.Id);
//                        if (tk == null)
//                        {
//                            var token = await Redis.db.SetLoginToken(User.Id);
//                            await Redis.db.RemoveLoginLog(User.Mobile);
//                            controllerObj?.HttpContext.SetCookieUserToken(token);
//                        }
//                    }
//                }
//            }

//            if (User == null || User.Menus == null)
//            {
//                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Authorize, MenuType.AuthorizeFilter, false, $"تقاضای دسترسی بدون لاگین!", null, FajrActionType.AccessDeniedError).Result;
//                if (IsAjaxRequest(context))
//                {
//                    context.HttpContext.Response.StatusCode = 401;
//                    context.Result = new EmptyResult();
//                }
//                else
//                    context.Result = new RedirectToActionResult("Index", "Authentication", new { area = "", RetUrl = url });
//                return;
//            }
//            #endregion

//            #region لاگین شخص دیگری با این یوزرنیم - اگر توکن کاربر با توکن درون ردیس یکی نباشد کاربر به صفحه لاگین منتقل می شود
//            var cookieToken = controllerObj?.HttpContext.GetCookieUserToken();
//            var redisToken = await Redis.db.GetLoginToken(User.Id);
//            // لاگ کردن مقادیر برای بررسی بیشتر
//            Console.WriteLine($"cookieToken: {cookieToken}");
//            Console.WriteLine($"redisToken: {redisToken}");
//            if (string.IsNullOrEmpty(cookieToken) || cookieToken != redisToken)
//            {

//                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Authorize, MenuType.AuthorizeFilter, false, $"کاربر {User.FullName} |    به دلیل ورود شخصی دیگر با این حساب کاربری، کاربر از حساب کاربری خود خارج شد با RedisToken = {redisToken} و coockieToken = {cookieToken} !", User.Id, FajrActionType.AccessDeniedError).Result;
//                if (IsAjaxRequest(context))
//                {
//                    context.HttpContext.Response.StatusCode = 401;
//                    context.Result = new EmptyResult();
//                }
//                else
//                {
//                    controllerObj?.HttpContext.Session.RemoveUser();
//                    context.Result = new RedirectToActionResult("Index", "Authentication", new { area = "", RetUrl = url, newlogin = "t" });
//                }
//                return;
//            }
//            #endregion

//            #region بررسی دسترسی کاربر درون سشن به بخش مورد نظر
//            var menu = HasPermission(context, User);

//            if (!isPublic && menu == null)
//            {
//                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Authorize, MenuType.AuthorizeFilter, false, $"کاربر {User.FullName} | کاربر به صفحه درخواست شده دسترسی ندارد!", User.Id, FajrActionType.AccessDeniedError).Result;
//                if (IsAjaxRequest(context))
//                {
//                    context.HttpContext.Response.StatusCode = 403;
//                    context.Result = new EmptyResult();
//                }
//                else
//                    context.Result = new RedirectToActionResult("Index", "Forbidden", new { area = "Admin" });
//                return;
//            }
//            #endregion

//            if (!isLoginWithFace)
//            {
//                #region آیا منو مورد نظر نیاز به احراز هویت مجدد دارد؟
//                if (menu != null && menu.NeedReAuthorize)
//                {
//                    if (!IsAjaxRequest(context))
//                    {
//                        var isAuthorized = Redis.db.UserHasReAuthorizeMenu(menu.Id, User.Id).Result;
//                        if (!isAuthorized)
//                            context.Result = new RedirectToActionResult("Index", "Authentication", new { area = "", RetUrl = url, mid = menu.Id });
//                    }
//                }
//                #endregion

//                #region آیا کاربر تغییر رمز را انجام داده؟
//                if (checkPasswordChange && !User.PasswordIsChanged)
//                {
//                    context.Result = new RedirectToActionResult("ChangePassword", "Profile", new { area = "Admin" });
//                    return;
//                }
//                #endregion
//            }

//            return;
//        }

//        #region بررسی دسترسی کاربر 
//        public MenuSessionDTO HasPermission(ActionExecutingContext context, UserSessionDTO User)
//        {
//            #region بررسی دسترسی به منو
//            #region اگر پارامترهای ادرس مشخص نشده باشد، دسترسی کاربر به ادرس درخواست شده بررسی می شود
//            var controllerObj = context.Controller as Controller;
//            if (area == null)
//            {
//                var _area = controllerObj?.RouteData.Values["area"]?.ToString()?.ToLower();
//                if (string.IsNullOrEmpty(_area))
//                    _area = null;
//                area = _area;
//            }
//            if (controller == null)
//            {
//                var _controller = controllerObj?.RouteData.Values["controller"]?.ToString()?.ToLower();
//                controller = _controller;
//            }
//            if (action == null)
//            {
//                var _action = controllerObj?.RouteData.Values["action"]?.ToString()?.ToLower();
//                action = _action;
//            }
//            #endregion

//            var menu = User.Menus.FirstOrDefault(x => x.Area == area && x.Controller == controller && x.Action == action);
//            return menu;
//            #endregion
//        }
//        #endregion

//        #region بررسی اینکه آیا درخواست از نوع اجکس است
//        public bool IsAjaxRequest(ActionExecutingContext context)
//        {
//            string requestedWith = context.HttpContext.Request.Headers["X-Requested-With"];
//            return requestedWith == "XMLHttpRequest";
//        }
//        #endregion
//    }
//}
using Domain.Enums;
using DTO.Menu;
using DTO.User;
using FajrLog.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Services.CookieServices;
using Services.RedisService;
using Services.SessionServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Filters
{
    /// <summary>
    /// بررسی وجود کاربر درون سشن به منظور لاگین
    /// </summary>

    public class UserAuthorize : ActionFilterAttribute
    {
        private bool isPublic;
        private bool checkPasswordChange;
        private string area;
        private string controller;
        private string action;

        /// <summary>
        /// بررسی دسترسی کاربر
        /// </summary>
        /// <param name="IsPublic">آیا ادرس مورد نظر عمومی است و همه به آن دسترسی دارند؟</param>
        public UserAuthorize(bool IsPublic = false, string? Area = null, string? Controller = null, string? Action = null, bool CheckPasswordChange = true)
        {
            isPublic = IsPublic;
            checkPasswordChange = CheckPasswordChange;
            area = Area?.ToLower();
            controller = Controller?.ToLower();
            action = Action?.ToLower();
        }



        public override void OnActionExecuted(ActionExecutedContext context)
        {
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var url = context.HttpContext.Request.Path.ToString()
                        + context.HttpContext.Request.QueryString;


            #region اگر ادمین درون سشن نباشد کاربر به صفحه لاگین ریدایرکت میشود
            var controllerObj = context.Controller as Controller;
            var Redis = controllerObj?.HttpContext.RequestServices.GetService<IRedisManager>();
            var User = controllerObj?.HttpContext.Session.GetUser();
            if (User == null || User.Menus == null)
            {
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Authorize, MenuType.AuthorizeFilter, false, $"تقاضای دسترسی بدون لاگین!", null, FajrActionType.AccessDeniedError).Result;
                if (IsAjaxRequest(context))
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new EmptyResult();
                }
                else
                    context.Result = new RedirectToActionResult("Index", "Authentication", new { area = "", RetUrl = url });
                return;
            }
            #endregion

            #region اجبار تغییر کلمه عبور در اولین ورود
            var currentArea = controllerObj?.RouteData.Values["area"]?.ToString();
            var currentController = controllerObj?.RouteData.Values["controller"]?.ToString();
            var currentAction = controllerObj?.RouteData.Values["action"]?.ToString();

            var isChangePasswordPage =
                string.Equals(currentArea, "Admin", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, "Profile", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentAction, "ChangePassword", StringComparison.OrdinalIgnoreCase);

            if (checkPasswordChange &&
                User.PasswordIsChanged == false &&
                isChangePasswordPage == false)
            {
                if (IsAjaxRequest(context))
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                    context.Result = new JsonResult(new
                    {
                        Status = false,
                        Message = "برای ادامه، ابتدا کلمه عبور خود را تغییر دهید.",
                        RedirectUrl = "/Admin/Profile/ChangePassword"
                    });
                }
                else
                {
                    context.Result = new RedirectToActionResult(
                        "ChangePassword",
                        "Profile",
                        new { area = "Admin" });
                }

                return;
            }
            #endregion


            return;
        }






        #region بررسی دسترسی کاربر 
        public MenuSessionDTO HasPermission(ActionExecutingContext context, UserSessionDTO User)
        {
            #region بررسی دسترسی به منو
            #region اگر پارامترهای ادرس مشخص نشده باشد، دسترسی کاربر به ادرس درخواست شده بررسی می شود
            var controllerObj = context.Controller as Controller;
            if (area == null)
            {
                var _area = controllerObj?.RouteData.Values["area"]?.ToString()?.ToLower();
                if (string.IsNullOrEmpty(_area))
                    _area = null;
                area = _area;
            }
            if (controller == null)
            {
                var _controller = controllerObj?.RouteData.Values["controller"]?.ToString()?.ToLower();
                controller = _controller;
            }
            if (action == null)
            {
                var _action = controllerObj?.RouteData.Values["action"]?.ToString()?.ToLower();
                action = _action;
            }
            #endregion

            var menu = User.Menus.FirstOrDefault(x => x.Area == area && x.Controller == controller && x.Action == action);
            return menu;
            #endregion
        }
        #endregion




        #region بررسی اینکه آیا درخواست از نوع اجکس است
        public bool IsAjaxRequest(ActionExecutingContext context)
        {
            string requestedWith = context.HttpContext.Request.Headers["X-Requested-With"];
            return requestedWith == "XMLHttpRequest";
        }
        #endregion



    }


}
