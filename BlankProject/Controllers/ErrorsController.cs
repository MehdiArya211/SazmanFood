using DocumentFormat.OpenXml.Math;
using Domain.Enums;
using DTO.Base;
using DTO.ExceptionHandling;
using FajrLog.Enum;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Services.RedisService;

namespace Food.Controllers
{

    /// <summary>
    /// مدیریت خطاها و تولید خروجی مناسب با استاتوس کد مناسب
    /// </summary>
    public class ErrorsController : Controller
    {
        private readonly string MainSiteUrl;
        private readonly IRedisManager Redis;
        public ErrorsController(IRedisManager _Redis, IConfiguration iConfig)
        {
            MainSiteUrl = iConfig.GetSection("MainSiteUrl").Value?.Trim();
            Redis = _Redis;
        }


        [HttpGet]
        [Route("error")]
        public IActionResult GetError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;
            var code = 500;
            string title = "خطا";

            if (exception is HttpStatusException httpException)
            {
                code = (int)httpException.Status;
                title = httpException.Title;
            }

            #region نوع لاگ فجر
            FajrActionType fajrActionType = FajrActionType.SystemError;
            //حملات xss
            if (Response.StatusCode == 406 || code == 406)
                fajrActionType = FajrActionType.XSSAttack; 
            #endregion

            Response.StatusCode = code;
            ViewBag.MainSiteUrl = MainSiteUrl;
            // اگر درخواست از نوع اجکس بود
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.ServerError, MenuType.ErrorPage, false, $"Error handler - ajax method GET | کد خطا : {code} | عنوان : {title} | شرح : {exception.Message}", null, fajrActionType).Result;
                return Json(new BaseResult(false, exception.Message));
            }
            else // درخواست غیر اجکس
            {
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.ServerError, MenuType.ErrorPage, false, $"Error handler - method GET | کد خطا : {code} | عنوان : {title} | شرح : {exception.Message}", null, fajrActionType).Result;
                return View("ErrorPage", new ErrorPageDTO(code.ToString(), title, exception.Message));
            }

        }



        [HttpPost]
        [Route("error")]
        public IActionResult PostError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;
            var code = 500;
            string title = "خطا";

            if (exception is HttpStatusException httpException)
            {
                code = (int)httpException.Status;
                title = httpException.Title;
            }

            #region نوع لاگ فجر
            FajrActionType fajrActionType = FajrActionType.SystemError;
            //حملات xss
            if (Response.StatusCode == 406 || code == 406)
                fajrActionType = FajrActionType.XSSAttack;
            #endregion

            Response.StatusCode = code;
            ViewBag.MainSiteUrl = MainSiteUrl;

            // اگر درخواست از نوع اجکس بود
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.ServerError, MenuType.ErrorPage, false, $"Error handler - ajax method POST | کد خطا : {code} | عنوان : {title} | شرح : {exception.Message}", null, fajrActionType).Result;
                return Json(new BaseResult(false, exception.Message));
            }
            else // درخواست غیر اجکس
            {
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.ServerError, MenuType.ErrorPage, false, $"Error handler - method POST | کد خطا : {code} | عنوان : {title} | شرح : {exception.Message}", null, fajrActionType).Result;
                return View("ErrorPage", new ErrorPageDTO(code.ToString(), title, exception.Message));
            }
        }

    }
}