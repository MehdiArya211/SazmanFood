using System.Text.Json;
using Domain.Entities.LogSystem;
using Domain.Enums;
using DTO.Base;
using DTO.ExceptionHandling;
using FajrLog.Enum;
using Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.RedisService;
using Services.SessionServices;

namespace Food.Controllers
{
    /// <summary>ثبت و نمایش امن خطاهای کنترل‌نشدهٔ برنامه</summary>
    public class ErrorsController : Controller
    {
        private const string DiagnosticCode = "APP_DIAGNOSTIC";
        private readonly string mainSiteUrl;
        private readonly IRedisManager redis;
        private readonly ApplicationContext db;
        private readonly ILogger<ErrorsController> logger;

        public ErrorsController(
            IRedisManager redisManager,
            IConfiguration configuration,
            ApplicationContext applicationContext,
            ILogger<ErrorsController> appLogger)
        {
            mainSiteUrl = configuration.GetSection("MainSiteUrl").Value?.Trim() ?? "";
            redis = redisManager;
            db = applicationContext;
            logger = appLogger;
        }

        [Route("error")]
        public async Task<IActionResult> Handle()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception == null)
                return StatusCode(StatusCodes.Status500InternalServerError, "خطای نامشخص رخ داده است.");

            var code = exception is HttpStatusException httpException
                ? (int)httpException.Status
                : StatusCodes.Status500InternalServerError;
            var title = exception is HttpStatusException typedException
                ? typedException.Title
                : "خطای سامانه";

            var diagnosticId = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
            var pathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var path = pathFeature?.Path ?? HttpContext.Request.Path.Value ?? "/";
            var endpoint = pathFeature?.Endpoint?.DisplayName ?? HttpContext.GetEndpoint()?.DisplayName;
            var user = HttpContext.Session.GetUser();
            var properties = new
            {
                DiagnosticId = diagnosticId,
                Path = path,
                Endpoint = endpoint,
                Method = HttpContext.Request.Method,
                TraceId = HttpContext.TraceIdentifier,
                UserId = user?.Id,
                Username = user?.Username,
                RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            try
            {
                db.SeriLog.Add(new SeriLog
                {
                    Message = $"خطای کنترل‌نشده در {path} (شناسه پیگیری: {diagnosticId})",
                    MessageTemplate = "{DiagnosticId} {Method} {Path} {Endpoint}",
                    Level = "Error",
                    TimeStamp = DateTime.UtcNow,
                    Exception = exception.ToString(),
                    Properties = JsonSerializer.Serialize(properties),
                    LogEvent = DiagnosticCode
                });
                await db.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception loggingException)
            {
                logger.LogError(loggingException,
                    "ذخیره خطای برنامه ناموفق بود. DiagnosticId={DiagnosticId}", diagnosticId);
            }

            try
            {
                var fajrType = code == 406 ? FajrActionType.XSSAttack : FajrActionType.SystemError;
                await redis.db.SetLog(redis.ContextAccessor, ActionType.ServerError, MenuType.ErrorPage, false,
                    $"خطای کنترل‌نشده | کد: {code} | مسیر: {path} | شناسه پیگیری: {diagnosticId}",
                    null, fajrType);
            }
            catch (Exception loggingException)
            {
                logger.LogWarning(loggingException,
                    "ثبت خطای برنامه در Redis ناموفق بود. DiagnosticId={DiagnosticId}", diagnosticId);
            }

            Response.StatusCode = code;
            ViewBag.MainSiteUrl = mainSiteUrl;
            var safeMessage = $"خطایی هنگام پردازش درخواست رخ داد. شناسه پیگیری: {diagnosticId}. " +
                              "مدیر سامانه می‌تواند جزئیات را در گزارش خطاها بررسی کند.";

            var wantsJson = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                            HttpContext.Request.Headers["Accept"].Any(value =>
                                value?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true);
            if (wantsJson)
                return Json(new BaseResult(false, safeMessage));

            return View("ErrorPage", new ErrorPageDTO(code.ToString(), title, safeMessage));
        }
    }
}
