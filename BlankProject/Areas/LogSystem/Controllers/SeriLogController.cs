using System.Globalization;
using System.Text.Json;
using BLL;
using BLL.Interface;
using DTO;
using Domain.Constants;
using Filters;
using Food.Areas.LogSystem.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.SessionServices;

namespace Food.Areas.LogSystem.Controllers
{
    [Area("LogSystem")]
    public class SeriLogController : Controller
    {
        private const string DiagnosticCode = "APP_DIAGNOSTIC";
        private readonly IDataTableManager dataTableManager;
        private readonly ISeriLogManager seriLogManager;
        private readonly ApplicationContext db;

        public SeriLogController(
            IDataTableManager _dataTableManager,
            ISeriLogManager _seriLogManager,
            ApplicationContext _db)
        {
            dataTableManager = _dataTableManager;
            seriLogManager = _seriLogManager;
            db = _db;
        }

        [UserAuthorize(Area: "LogSystem", Controller: "SeriLog", Action: "index")]
        public IActionResult Index() => View();

        [HttpPost]
        [UserAuthorize(Area: "LogSystem", Controller: "SeriLog", Action: "index")]
        public ActionResult GetList(SeriLogFilter filters)
        {
            var searchModel = dataTableManager.GetSearchModel();
            var model = seriLogManager.GetDataTableDTO(searchModel, filters);
            return Json(model);
        }

        [HttpGet]
        [UserAuthorize(IsPublic: true)]
        public async Task<IActionResult> Diagnostics(string? q, int page = 1, long? id = null)
        {
            var user = HttpContext.Session.GetUser();
            if (user == null)
                return RedirectToAction("Index", "Authentication", new { area = "" });

            // این برنامه مجوزدهی را با Session و UserAuthorize انجام می‌دهد
            // و Authentication Scheme برای Forbid() ندارد. مدیر سامانه نیز
            // باید حتی پیش از تازه‌سازی منوهای سشن به صفحهٔ گزارش دسترسی داشته باشد.
            if (user.RoleId != RoleConstant.Admin &&
                !user.HasMenu("logsystem", "serilog", "diagnostics"))
                return RedirectToAction("Index", "Forbidden", new { area = "Admin" });

            var query = db.SeriLog.AsNoTracking().Where(x => x.LogEvent == DiagnosticCode);
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(x => x.Message.Contains(term) ||
                                         (x.Exception != null && x.Exception.Contains(term)) ||
                                         (x.Properties != null && x.Properties.Contains(term)));
            }

            var total = await query.CountAsync();
            page = Math.Max(1, page);
            const int pageSize = 30;
            var logs = await query.OrderByDescending(x => x.TimeStamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new DiagnosticLogViewModel
            {
                Query = q ?? "",
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Rows = logs.Select(ToRow).ToList()
            };

            if (id.HasValue)
            {
                var selected = await db.SeriLog.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id.Value && x.LogEvent == DiagnosticCode);
                if (selected != null)
                    ViewBag.Selected = ToRow(selected);
            }

            return View(model);
        }

        private static DiagnosticLogRow ToRow(Domain.Entities.LogSystem.SeriLog log)
        {
            JsonDocument? json = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(log.Properties))
                    json = JsonDocument.Parse(log.Properties);
            }
            catch (JsonException) { }

            string Property(string key)
            {
                if (json == null || json.RootElement.ValueKind != JsonValueKind.Object ||
                    !json.RootElement.TryGetProperty(key, out var value))
                    return "";
                return value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : value.ToString();
            }

            var exception = log.Exception ?? "";
            var separator = exception.IndexOf(':');
            var type = separator > 0 ? exception[..separator].Trim() : "خطای ثبت‌شده";
            var message = separator > 0
                ? exception[(separator + 1)..].Split(" at ", StringSplitOptions.None)[0].Trim()
                : exception.Split(" at ", StringSplitOptions.None)[0].Trim();

            var timestamp = DateTime.SpecifyKind(log.TimeStamp, DateTimeKind.Utc);
            try
            {
                var zone = TimeZoneInfo.FindSystemTimeZoneById(
                    OperatingSystem.IsWindows() ? "Iran Standard Time" : "Asia/Tehran");
                timestamp = TimeZoneInfo.ConvertTimeFromUtc(timestamp, zone);
            }
            catch
            {
                timestamp = timestamp.AddHours(3.5);
            }

            var persian = new PersianCalendar();
            var weekdays = new[] { "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" };
            var date = $"{weekdays[(int)timestamp.DayOfWeek]} " +
                       $"{persian.GetYear(timestamp):0000}/{persian.GetMonth(timestamp):00}/{persian.GetDayOfMonth(timestamp):00} - {timestamp:HH:mm:ss}";

            var row = new DiagnosticLogRow
            {
                Id = log.Id,
                Reference = Property("DiagnosticId"),
                Message = log.Message ?? "خطای سامانه",
                Level = log.Level ?? "Error",
                Path = Property("Path"),
                Method = Property("Method"),
                Endpoint = Property("Endpoint"),
                TraceId = Property("TraceId"),
                Username = Property("Username"),
                ExceptionType = type,
                ExceptionMessage = message,
                StackTrace = exception,
                PersianDate = date
            };
            json?.Dispose();

            (row.Meaning, row.WhatToCheck) = Explain(type);
            return row;
        }

        private static (string Meaning, string Checks) Explain(string type)
        {
            if (type.Contains("SqlException", StringComparison.OrdinalIgnoreCase))
                return ("در اجرای درخواست پایگاه داده خطا رخ داده است.",
                    "اتصال و دسترسی بانک، وجود جدول/ستون، محدودیت‌های کلید و متن کامل خطا را بررسی کنید.");
            if (type.Contains("DbUpdate", StringComparison.OrdinalIgnoreCase))
                return ("ذخیره یا ویرایش اطلاعات در بانک انجام نشده است.",
                    "فیلدهای اجباری، مقادیر ارسالی، کلیدهای خارجی و خطای داخلی را بررسی کنید.");
            if (type.Contains("HttpRequestException", StringComparison.OrdinalIgnoreCase) ||
                type.Contains("TaskCanceledException", StringComparison.OrdinalIgnoreCase))
                return ("ارتباط با سرویس بیرونی برقرار نشده یا پاسخ به‌موقع نرسیده است.",
                    "آدرس سرویس، شبکه، گواهی TLS، زمان پاسخ و لاگ سرویس مقصد را بررسی کنید.");
            if (type.Contains("NullReferenceException", StringComparison.OrdinalIgnoreCase))
                return ("کد از شیئی استفاده کرده که مقداردهی نشده است.",
                    "مسیر صفحه و عملیات، داده‌های ورودی و اولین خط پروژه در Stack Trace را بررسی کنید.");
            if (type.Contains("InvalidOperationException", StringComparison.OrdinalIgnoreCase))
                return ("عملیات در وضعیت فعلی قابل انجام نبوده است.",
                    "پیام کامل، وضعیت داده/نشست و اولین خط Stack Trace متعلق به پروژه را بررسی کنید.");
            if (type.Contains("UnauthorizedAccessException", StringComparison.OrdinalIgnoreCase))
                return ("برنامه برای دسترسی به فایل یا منبعی مجوز کافی ندارد.",
                    "مجوز حساب اجرای IIS و دسترسی پوشه، بانک یا سرویس مقصد را بررسی کنید.");
            return ("در اجرای درخواست خطایی رخ داده است؛ جزئیات فنی ثبت شده.",
                "پیام خطا و اولین خط Stack Trace مربوط به پروژه را بررسی کنید.");
        }
    }
}
