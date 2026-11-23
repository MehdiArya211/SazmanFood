using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BLL;
using Domain.Constants;
using Domain.Entities.Garrison;
using Domain.Enums;
using DTO.Entities.FoodReservation;
using Filters;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.SessionServices;
using Utilities.Extentions;

namespace Food.Areas.FoodPlan.Controllers
{
    [Area("FoodPlan")]
    public class ReservationReportsController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly ISession _session;
        private static readonly PersianCalendar Persian = new PersianCalendar();

        public ReservationReportsController(ApplicationContext db, IHttpContextAccessor accessor)
        {
            _db = db;
            _session = accessor.HttpContext!.Session;
        }

        [HttpGet]
        [UserAuthorize(Area: "FoodPlan", Controller: "ReservationReports", Action: "Daily")]
        public IActionResult Daily(DateTime? fromDate, DateTime? toDate, long? mealId)
            => Report("daily", fromDate, toDate, mealId, null, "daily");

        [HttpGet]
        [UserAuthorize(Area: "FoodPlan", Controller: "ReservationReports", Action: "Calendar")]
        public IActionResult Calendar(DateTime? fromDate, DateTime? toDate, long? mealId)
            => Report("calendar", fromDate, toDate, mealId, null, "daily");

        [HttpGet]
        [UserAuthorize(Area: "FoodPlan", Controller: "ReservationReports", Action: "ByFood")]
        public IActionResult ByFood(DateTime? fromDate, DateTime? toDate, long? mealId, long? foodId, string groupBy = "daily")
            => Report("food", fromDate, toDate, mealId, foodId, groupBy);

        private IActionResult Report(string kind, DateTime? fromDate, DateTime? toDate, long? mealId, long? foodId, string groupBy)
        {
            var user = _session.GetUser();
            if (user?.IsEnabled != true || user.RoleId != RoleConstant.Admin)
                return Forbid();

            var current = FoodTimingRules.GetIranNow().Date;
            var firstDay = Persian.ToDateTime(Persian.GetYear(current), Persian.GetMonth(current), 1, 0, 0, 0, 0);
            var defaultEnd = firstDay.AddDays(Persian.GetDaysInMonth(Persian.GetYear(current), Persian.GetMonth(current)) - 1);
            var startDate = (fromDate ?? firstDay).Date;
            var to = (toDate ?? defaultEnd).Date;

            if (to < startDate || (to - startDate).TotalDays > 366)
            {
                ModelState.AddModelError("", "بازه تاریخ باید مرتب باشد و حداکثر ۳۶۷ روز را شامل شود.");
                return View("Index", new ReservationReportDTO
                {
                    Kind = kind, Title = GetTitle(kind), GroupBy = groupBy,
                    FromDate = startDate, ToDate = to, MealId = mealId, FoodId = foodId
                });
            }

            if (groupBy != "daily" && groupBy != "weekly" && groupBy != "monthly")
                groupBy = "daily";

            var model = new ReservationReportDTO
            {
                Kind = kind, Title = GetTitle(kind), GroupBy = groupBy,
                FromDate = startDate, ToDate = to, MealId = mealId, FoodId = foodId,
                Meals = _db.Meal.AsNoTracking().OrderBy(x => x.SortName)
                    .Select(x => new ReservationReportOptionDTO { Id = x.Id, Title = x.Title }).ToList(),
                Foods = _db.Foods.AsNoTracking().Where(x => x.IsDeleted != true)
                    .OrderBy(x => x.Title)
                    .Select(x => new ReservationReportOptionDTO { Id = x.Id, Title = x.Title }).ToList()
            };

            // تاریخ مصرف ژتون = شروع هفته + شماره روز ثبت‌شده در جزئیات رزرو.
            // CreatedAt زمان ثبت سفارش است و برای گزارش روز غذا کاربرد ندارد.
            var raw = (
                from detail in _db.FoodReserveDetails.AsNoTracking()
                join reserve in _db.FoodReserves.AsNoTracking() on detail.FoodReserveId equals reserve.Id
                join account in _db.Users.AsNoTracking() on reserve.UserId equals account.Id into accounts
                from account in accounts.DefaultIfEmpty()
                where reserve.WeekStartDate < to.AddDays(1) &&
                      reserve.WeekEndDate >= startDate &&
                      (!mealId.HasValue || detail.MealId == mealId.Value) &&
                      (kind != "food" || !foodId.HasValue || detail.MainFoodId == foodId.Value)
                select new
                {
                    reserve.WeekStartDate,
                    DayCode = detail.Day.Code,
                    detail.MealId,
                    MealTitle = detail.Meal.Title,
                    detail.MainFoodId,
                    FoodTitle = detail.MainFood != null ? detail.MainFood.Title : null,
                    PersonId = account != null ? account.PersonId : null,
                    PersonCode = account != null ? account.PersonCode : null
                }).ToList();

            var items = raw.Where(x => x.DayCode >= 1 && x.DayCode <= 7)
                .Select(x => new ReservationItem
                {
                    Date = x.WeekStartDate.Date.AddDays(x.DayCode - 1),
                    MealId = x.MealId,
                    Meal = x.MealTitle ?? "نامشخص",
                    FoodId = x.MainFoodId,
                    Food = x.FoodTitle ?? "بدون غذای اصلی",
                    PersonId = x.PersonId,
                    PersonCode = x.PersonCode
                })
                .Where(x => x.Date >= startDate && x.Date <= to).ToList();

            SetPersonalTypes(items);

            if (kind == "daily")
                model.Rows = items.GroupBy(x => new { x.Date, x.MealId, x.Meal })
                    .OrderBy(x => x.Key.Date).ThenBy(x => x.Key.MealId)
                    .Select(x => MakeRow(FormatDate(x.Key.Date), x.Key.Meal, "", x))
                    .ToList();
            else if (kind == "calendar")
                model.Rows = items.GroupBy(x => new
                    {
                        Year = Persian.GetYear(x.Date), Month = Persian.GetMonth(x.Date),
                        Day = x.Date.DayOfWeek, x.MealId, x.Meal
                    })
                    .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month)
                    .ThenBy(x => ((int)x.Key.Day + 1) % 7).ThenBy(x => x.Key.MealId)
                    .Select(x => MakeRow(
                        x.Key.Year + "/" + x.Key.Month.ToString("00") + " - " + Weekday(x.Key.Day),
                        x.Key.Meal, "", x))
                    .ToList();
            else
                model.Rows = items.GroupBy(x => new
                    {
                        Period = PeriodStart(x.Date, groupBy), x.FoodId, x.Food, x.MealId, x.Meal
                    })
                    .OrderBy(x => x.Key.Period).ThenBy(x => x.Key.Food).ThenBy(x => x.Key.MealId)
                    .Select(x => MakeRow(PeriodLabel(x.Key.Period, groupBy), x.Key.Meal, x.Key.Food, x))
                    .ToList();

            return View("Index", model);
        }

        private void SetPersonalTypes(List<ReservationItem> items)
        {
            var ids = items.Where(x => x.PersonId.HasValue).Select(x => x.PersonId!.Value).Distinct().ToArray();
            var codes = items.Where(x => x.PersonCode.HasValue).Select(x => x.PersonCode!.Value).Distinct().ToArray();
            var persons = _db.Set<Person>().AsNoTracking()
                .Where(x => ids.Contains(x.Id) || (x.PersonCode.HasValue && codes.Contains(x.PersonCode.Value)))
                .Select(x => new { x.Id, x.PersonCode, x.PersonTypeId })
                .ToList();
            var types = _db.PersonalType.AsNoTracking().ToDictionary(x => x.Id, x => x.Code);
            var byId = persons.ToDictionary(x => x.Id, x => x.PersonTypeId);
            var byCode = persons.Where(x => x.PersonCode.HasValue)
                .GroupBy(x => x.PersonCode!.Value)
                .ToDictionary(x => x.Key, x => x.First().PersonTypeId);

            var missingCodes = items.Where(x => x.PersonCode.HasValue &&
                    (!x.PersonId.HasValue || !byId.ContainsKey(x.PersonId.Value)) &&
                    !byCode.ContainsKey(x.PersonCode.Value))
                .Select(x => x.PersonCode!.Value.ToString()).Distinct().ToArray();

            // حساب‌های ساخته‌شده از اکسل ممکن است هنوز به جدول Person وصل نباشند.
            var quotaTypes = _db.Set<Domain.Entities.UnitQuotaPerson>().AsNoTracking()
                .Where(x => x.IsDeleted != true && x.PersonCode != null && missingCodes.Contains(x.PersonCode))
                .OrderByDescending(x => x.CreateDate)
                .Select(x => new { x.PersonCode, x.PersonalTypeId })
                .ToList()
                .GroupBy(x => x.PersonCode)
                .ToDictionary(x => x.Key, x => x.First().PersonalTypeId);

            foreach (var item in items)
            {
                long typeId = 0;
                if (item.PersonId.HasValue && byId.TryGetValue(item.PersonId.Value, out var personType))
                    typeId = personType;
                else if (item.PersonCode.HasValue && byCode.TryGetValue(item.PersonCode.Value, out personType))
                    typeId = personType;
                else if (item.PersonCode.HasValue &&
                         quotaTypes.TryGetValue(item.PersonCode.Value.ToString(), out personType))
                    typeId = personType;

                item.Type = types.TryGetValue(typeId, out var code) ? code : 0;
            }
        }

        private static ReservationReportRowDTO MakeRow(string period, string meal, string food, IEnumerable<ReservationItem> items)
        {
            var list = items.ToList();
            return new ReservationReportRowDTO
            {
                Period = period, Meal = meal, Food = food,
                Official = list.Count(x => x.Type == PersonalTypeCodes.Official),
                Duty = list.Count(x => x.Type == PersonalTypeCodes.Duty),
                Unknown = list.Count(x => x.Type != PersonalTypeCodes.Official && x.Type != PersonalTypeCodes.Duty)
            };
        }

        private static DateTime PeriodStart(DateTime date, string groupBy)
        {
            if (groupBy == "weekly")
                return date.AddDays(-(((int)date.DayOfWeek + 1) % 7));
            if (groupBy == "monthly")
                return Persian.ToDateTime(Persian.GetYear(date), Persian.GetMonth(date), 1, 0, 0, 0, 0);
            return date;
        }

        private static string PeriodLabel(DateTime start, string groupBy)
        {
            if (groupBy == "monthly")
                return Persian.GetYear(start) + "/" + Persian.GetMonth(start).ToString("00");
            return groupBy == "weekly"
                ? FormatDate(start) + " تا " + FormatDate(start.AddDays(6))
                : FormatDate(start);
        }

        private static string FormatDate(DateTime date)
            => date.ToPersianDateTime().ToString().Split(' ')[0];

        private static string Weekday(DayOfWeek day) => day switch
        {
            DayOfWeek.Saturday => "شنبه",
            DayOfWeek.Sunday => "یکشنبه",
            DayOfWeek.Monday => "دوشنبه",
            DayOfWeek.Tuesday => "سه‌شنبه",
            DayOfWeek.Wednesday => "چهارشنبه",
            DayOfWeek.Thursday => "پنجشنبه",
            _ => "جمعه"
        };

        private static string GetTitle(string kind) => kind switch
        {
            "daily" => "گزارش روزانه رزرو غذا",
            "calendar" => "گزارش رزرو به تفکیک روز هفته و ماه",
            _ => "گزارش رزرو به تفکیک غذا"
        };

        private sealed class ReservationItem
        {
            public DateTime Date { get; set; }
            public long MealId { get; set; }
            public string Meal { get; set; } = "";
            public long? FoodId { get; set; }
            public string Food { get; set; } = "";
            public long? PersonId { get; set; }
            public int? PersonCode { get; set; }
            public int Type { get; set; }
        }
    }
}
