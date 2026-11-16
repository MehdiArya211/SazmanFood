using BLL.Interface;
using Domain.Constants;
using DTO.Base;
using Microsoft.Extensions.Options;
using System.Drawing.Printing;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.SessionServices;

namespace Food.Areas.Admin.Controllers;

/// <summary>
/// کنترل مرکزی سیاست رزرو غذا. تنها مدیر سیستم می‌تواند وضعیت را تغییر دهد.
/// </summary>
[Area("Admin")]
[UserAuthorize(IsPublic: true)]
public class QuotaControlController : Controller
{
    private readonly IConstantManager _constantManager;
    private readonly IUnitQuotaManager _unitQuotaManager;
    private readonly ILogger<QuotaControlController> _logger;
    private readonly PrinterOptions _printerOptions;

    public QuotaControlController(
        IConstantManager constantManager,
        IUnitQuotaManager unitQuotaManager,
        ILogger<QuotaControlController> logger,
        IOptions<PrinterOptions> printerOptions)
    {
        _constantManager = constantManager;
        _unitQuotaManager = unitQuotaManager;
        _logger = logger;
        _printerOptions = printerOptions?.Value ?? new PrinterOptions();
    }

    [HttpGet]
    public IActionResult Index()
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.RoleId != RoleConstant.Admin || !user.IsEnabled)
            return Forbid();

        ViewBag.FreeReservationEnabled =
            _constantManager.IsFreeFoodReservationEnabled();
        try
        {
            ViewBag.TimingPolicy = _constantManager.GetFoodTimingPolicy();
        }
        catch (Exception ex)
        {
            // در صورت خرابی مقدار تنظیمات، مدیر باید همچنان بتواند
            // صفحه را باز کند و با ذخیره مجدد، داده را اصلاح نماید.
            _logger.LogError(ex, "تنظیمات زمان‌بندی غذا نامعتبر است.");
            ViewBag.TimingPolicy = new FoodTimingPolicyDTO();
            ViewBag.TimingPolicyError =
                "تنظیمات ذخیره‌شده معتبر نیست. برای اصلاح، مقادیر این بخش را بررسی و دوباره ذخیره کنید.";
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save(bool enabled)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.RoleId != RoleConstant.Admin || !user.IsEnabled)
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                Status = false,
                Message = "فقط مدیر سیستم مجاز به تغییر سیاست سهمیه‌بندی است."
            });

        var previousValue = _constantManager.IsFreeFoodReservationEnabled();
        if (previousValue == enabled)
        {
            return Json(new
            {
                Status = true,
                Enabled = enabled,
                Message = "وضعیت رزرو غذا بدون تغییر باقی ماند."
            });
        }

        var result = _constantManager.SetFreeFoodReservationEnabled(enabled);
        if (!result.Status)
        {
            _logger.LogError(
                "ذخیره تنظیم رزرو آزاد ناموفق بود. UserId: {UserId}, RequestedEnabled: {Enabled}, Message: {Message}",
                user.Id, enabled, result.Message);
            return Json(new { Status = false, Message = result.Message });
        }

        _logger.LogWarning(
            "سیاست رزرو غذا تغییر کرد. AdminUserId: {UserId}, Previous: {Previous}, New: {Current}, Time: {Time}",
            user.Id, previousValue, enabled, DateTimeOffset.Now);

        return Json(new
        {
            Status = true,
            Enabled = enabled,
            Message = enabled
                ? "رزرو آزاد فعال شد. اکنون کاربران واردشده بدون سهمیه فردی می‌توانند غذا رزرو کنند."
                : "رزرو آزاد غیرفعال شد. از این لحظه ثبت غذا فقط طبق سهمیه فردی مجاز است."
        });
    }

    /// <summary>
    /// فقط مدیر می‌تواند برای سهمیه‌های ثبت‌شده قبلی حساب اولیه بسازد.
    /// هیچ کلمه عبور موجودی در این مسیر تغییر داده نمی‌شود.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProvisionMissingUsers(long afterId = 0)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.RoleId != RoleConstant.Admin || !user.IsEnabled)
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                Status = false,
                Message = "فقط مدیر سامانه می‌تواند حساب اولیه پرسنل را آماده کند."
            });

        if (afterId < 0)
            return BadRequest(new { Status = false, Message = "شناسه ادامه معتبر نیست." });

        try
        {
            ReservationUserProvisioningBatchDTO result =
                _unitQuotaManager.ProvisionUsersFromExistingQuotas(afterId, 100);

            return Json(new
            {
                Status = true,
                result.Examined,
                result.Created,
                result.AlreadyExists,
                result.Failed,
                result.LastId,
                result.HasMore,
                result.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "آماده‌سازی حساب‌های ثبت‌شده سهمیه با خطا مواجه شد. AdminUserId: {AdminUserId}, AfterId: {AfterId}",
                user.Id, afterId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Status = false,
                Message = "آماده‌سازی حساب‌ها با خطا مواجه شد؛ لاگ سرور را بررسی کنید."
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveTimingPolicy(FoodTimingPolicyDTO policy)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.RoleId != RoleConstant.Admin || !user.IsEnabled)
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                Status = false,
                Message = "فقط مدیر سیستم مجاز به تغییر محدودیت‌های زمانی است."
            });

        var result = _constantManager.SaveFoodTimingPolicy(policy);
        if (!result.Status)
        {
            _logger.LogWarning(
                "ذخیره محدودیت‌های زمان‌بندی غذا رد شد. AdminUserId: {UserId}, Error: {Message}",
                user.Id, result.Message);

            return BadRequest(new
            {
                Status = false,
                Message = result.Message
            });
        }

        _logger.LogWarning(
            "محدودیت‌های زمانی غذا توسط مدیر ثبت شد. AdminUserId: {UserId}, PrintEnabled: {PrintEnabled}, PrintStart: {PrintStart}, ReservationEnabled: {ReservationEnabled}, Days: {Days}",
            user.Id,
            policy.PrintTimeEnabled,
            policy.PrintStartTime,
            policy.ReservationDeadlineEnabled,
            string.Join(";", policy.Days.OrderBy(x => x.DayOfWeek)
                .Select(x => $"{x.DayOfWeek}:{x.DaysBefore}@{x.CutoffTime}")));

        return Json(new
        {
            Status = true,
            Message = "تنظیمات ساعت چاپ ژتون و مهلت رزرو غذا با موفقیت ذخیره شد."
        });
    }
    /// <summary>
    /// تشخیص محل اجرای چاپ و قابل مشاهده بودن چاپگر از داخل حساب IIS.
    /// فقط به مدیر نمایش داده می‌شود؛ این تست چاپ فیزیکی نیست.
    /// </summary>
    [HttpGet]
    public IActionResult PrinterDiagnostics()
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.RoleId != RoleConstant.Admin || !user.IsEnabled)
            return Forbid();

        var name = _printerOptions.FoodTokenPrinterName?.Trim() ?? string.Empty;
        var info = new PrinterDiagnosticsDTO
        {
            ServerName = Environment.MachineName,
            ProcessUser = Environment.UserDomainName + "\\" + Environment.UserName,
            ConfiguredPrinterName = name,
            PrinterConfigured = name.Length > 0,
            IsWindows = OperatingSystem.IsWindows()
        };

        if (!info.IsWindows)
        {
            info.Error = "چاپ مستقیم با System.Drawing.Printing روی این سیستم‌عامل پشتیبانی نمی‌شود.";
            return View(info);
        }

        try
        {
            info.InstalledPrinters = PrinterSettings.InstalledPrinters
                .Cast<string>()
                .OrderBy(x => x)
                .ToList();

            info.PrinterVisible = name.Length > 0 && info.InstalledPrinters.Any(
                p => string.Equals(p, name, StringComparison.OrdinalIgnoreCase));

            if (!info.PrinterConfigured)
            {
                info.Error = "نام پرینتر سرور در Printer:FoodTokenPrinterName مشخص نشده است.";
            }
            else if (!info.PrinterVisible)
            {
                info.Error = "پرینتر تنظیم‌شده برای حساب اجرای وب قابل مشاهده نیست؛ نصب چاپگر برای کاربر Administrator کافی نیست.";
            }
            else
            {
                using var printDocument = new PrintDocument();
                printDocument.PrinterSettings.PrinterName = name;
                info.PrinterValid = printDocument.PrinterSettings.IsValid;
                if (!info.PrinterValid)
                {
                    info.Error = "درایور یا چاپگر از دید فرآیند IIS معتبر نیست (IsValid=false).";
                }
                else
                {
                    var paper = printDocument.DefaultPageSettings.PaperSize;
                    if (paper != null)
                    {
                        info.PaperName = paper.PaperName;
                        info.PaperWidthHundredthsInch = paper.Width;
                        info.PaperHeightHundredthsInch = paper.Height;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            info.Error = "خواندن تنظیمات چاپگر در فرآیند وب ناموفق بود.";
            _logger.LogError(ex,
                "خطا در عیب‌یابی چاپگر. Server: {Server}, ProcessUser: {ProcessUser}, ConfiguredPrinter: {Printer}",
                info.ServerName, info.ProcessUser, name);
        }

        return View(info);
    }

}
