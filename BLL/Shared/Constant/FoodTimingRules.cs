using System;
using System.Globalization;
using System.Linq;
using DTO.Base;

namespace BLL
{
    /// <summary>
    /// قوانین زمانی رزرو و چاپ بر اساس ساعت رسمی ایران.
    /// منطق مستقل از ساعت و منطقه زمانی مرورگر و سیستم میزبان است.
    /// </summary>
    public static class FoodTimingRules
    {
        private static readonly TimeZoneInfo IranTimeZone = ResolveIranTimeZone();

        public static DateTime GetIranNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IranTimeZone);
        }

        public static bool TryReadTime(string value, out TimeSpan time)
        {
            return TimeSpan.TryParseExact(
                    value,
                    @"hh\:mm",
                    CultureInfo.InvariantCulture,
                    out time) &&
                   time >= TimeSpan.Zero &&
                   time < TimeSpan.FromDays(1);
        }

        public static bool CanReserve(
            FoodTimingPolicyDTO policy,
            DateTime targetDate,
            DateTime nowIran,
            out string reason)
        {
            reason = null;
            if (policy == null || !policy.ReservationDeadlineEnabled)
                return true;

            var day = policy.Days?.SingleOrDefault(
                d => d.DayOfWeek == (int)targetDate.DayOfWeek);
            if (day == null ||
                day.DaysBefore < 0 ||
                day.DaysBefore > 7 ||
                !TryReadTime(day.CutoffTime, out var cutoffTime))
            {
                reason = "تنظیمات مهلت رزرو این روز معتبر نیست. با مدیریت تماس بگیرید.";
                return false;
            }

            var deadline = targetDate.Date
                .AddDays(-day.DaysBefore)
                .Add(cutoffTime);

            if (nowIran <= deadline)
                return true;

            reason = $"مهلت رزرو غذای تاریخ {targetDate:yyyy/MM/dd} " +
                     $"در {deadline:yyyy/MM/dd} ساعت {day.CutoffTime} به پایان رسیده است.";
            return false;
        }

        public static bool CanPrint(
            FoodTimingPolicyDTO policy,
            DateTime mealDate,
            DateTime nowIran,
            out string reason)
        {
            reason = null;
            if (policy == null || !policy.PrintTimeEnabled)
                return true;

            if (!TryReadTime(policy.PrintStartTime, out var start))
            {
                reason = "ساعت شروع چاپ ژتون معتبر نیست. با مدیریت تماس بگیرید.";
                return false;
            }

            // چاپ زودهنگام ژتون روز آینده در حالت زمان‌بندی مجاز نیست.
            if (mealDate.Date > nowIran.Date)
            {
                reason = "ژتون این روز فقط در همان تاریخ و پس از ساعت " +
                         policy.PrintStartTime + " قابل چاپ است.";
                return false;
            }

            if (mealDate.Date < nowIran.Date)
            {
                reason = "زمان چاپ ژتون این روز گذشته است.";
                return false;
            }

            if (nowIran.TimeOfDay < start)
            {
                reason = "چاپ ژتون امروز از ساعت " +
                         policy.PrintStartTime + " امکان‌پذیر است.";
                return false;
            }

            return true;
        }

        private static TimeZoneInfo ResolveIranTimeZone()
        {
            foreach (var zone in new[] { "Asia/Tehran", "Iran Standard Time" })
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(zone);
                }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }

            // ایران از ۱۴۰۲ تغییر ساعت فصلی ندارد.
            // در میزبان‌های فاقد tzdata از اختلاف ثابت +03:30 استفاده می‌شود.
            return TimeZoneInfo.CreateCustomTimeZone(
                "Iran Fixed +03:30",
                TimeSpan.FromMinutes(210),
                "Iran Fixed +03:30",
                "Iran Fixed +03:30");
        }
    }
}
