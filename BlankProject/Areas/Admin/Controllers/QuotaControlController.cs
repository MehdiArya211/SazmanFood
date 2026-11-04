using BLL.Interface;
using Domain.Constants;
using DTO.Base;
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
    private readonly ILogger<QuotaControlController> _logger;

    public QuotaControlController(
        IConstantManager constantManager,
        ILogger<QuotaControlController> logger)
    {
        _constantManager = constantManager;
        _logger = logger;
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
}
