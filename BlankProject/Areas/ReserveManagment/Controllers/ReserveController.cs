using BLL;
using BLL.FoodManag.FoodBL;
using BLL.Interface;
using BLL.ReserveManagment;
using Domain.Entities.FoodManage;
using Domain.Entities.FoodReservation;
using DTO.Entities;
using DTO.Entities.FoodReservation;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.RedisService;
using Services.SessionServices;

namespace Food.Areas.ReserveManagment.Controllers
{
    /// <summary>
    /// مدیریت رزرو غذا - غذا
    /// </summary>
    [Area("ReserveManagment")]
    [UserAuthorize(IsPublic: true)]
    public class ReserveController : Controller
    {
        private readonly IFoodManager _foodManager;
        private readonly IFoodPlanDayManager _foodPlanDayManager;
        private readonly IMealManager _mealManager;
        private readonly IFoodReserveManager _foodReserveManager;
        private readonly IFoodReserveDetailManager _foodReserveDetailManager;
        private readonly IRedisManager _redis;
        private readonly IUnitQuotaManager _unitQuotaManager;
        private readonly IConstantManager _constantManager;

        public ReserveController(IFoodPlanDayManager foodPlanDayManager, IMealManager mealManager, IFoodManager foodManager
            , IFoodReserveManager foodReserveManager, IRedisManager redis,
            IFoodReserveDetailManager foodReserveDetailManager,
            IUnitQuotaManager unitQuotaManager,
            IConstantManager constantManager)
        {
            _foodPlanDayManager = foodPlanDayManager;
            _foodManager = foodManager;
            _mealManager = mealManager;
            _foodReserveManager = foodReserveManager;
            _redis = redis;
            _foodReserveDetailManager = foodReserveDetailManager;
            _unitQuotaManager = unitQuotaManager;
            _constantManager = constantManager;
        }

        public IActionResult Index()
        {
            var user = HttpContext.Session.GetUser();
            if (user == null)
                return RedirectToAction("IndexZP", "Authentication", new { area = "" });

            long currentUserId = user.Id;

            // --- محاسبه محدوده زمانی هفته جاری ---
            var now = FoodTimingRules.GetIranNow();
            // محاسبه شروع هفته (فرض: شروع هفته از شنبه)
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Saturday)) % 7;
            DateTime weekStartDate = now.AddDays(-1 * diff).Date;
            DateTime weekEndDate = weekStartDate.AddDays(6).Date;

            var personalCode =
                user.PersonCode?.ToString() ??
                user.Username;

            var nationalCode =
                user.NationalCode?.ToString();

            var freeReservationEnabled =
                _constantManager.IsFreeFoodReservationEnabled();

            var reservableSlots = freeReservationEnabled
                ? GetOpenReservableSlots(weekStartDate)
                : _unitQuotaManager
                    .GetReservableSlots(
                        user.PersonId,
                        personalCode,
                        nationalCode,
                        user.OmdOrgId,
                        weekStartDate,
                        weekEndDate)
                    .ToHashSet();

            var timingPolicy = _constantManager.GetFoodTimingPolicy();
            var slotsBeforeTiming = reservableSlots.Count;
            reservableSlots.RemoveWhere(slot =>
                !FoodTimingRules.CanReserve(
                    timingPolicy, slot.Date, now, out _));

            ViewBag.ReservationDeadlineEnabled = timingPolicy.ReservationDeadlineEnabled;
            ViewBag.ClosedReservationSlotsCount =
                slotsBeforeTiming - reservableSlots.Count;

            ViewBag.FreeReservationEnabled = freeReservationEnabled;
            ViewBag.WeekStartDate = weekStartDate;
            ViewBag.AllowedReservationSlots = reservableSlots
                .Select(x => $"{x.Date:yyyyMMdd}_{x.MealId}")
                .ToHashSet();
            ViewBag.HasReservationQuota = reservableSlots.Count > 0;

            if (reservableSlots.Count == 0)
            {
                ViewBag.QuotaError = (timingPolicy.ReservationDeadlineEnabled &&
                                           slotsBeforeTiming > 0)
                    ? "مهلت رزرو روزها و وعده‌های مجاز این هفته به پایان رسیده است."
                    : freeReservationEnabled
                        ? "برای این هفته برنامه غذایی قابل رزرو تعریف نشده است."
                        : "سهمیه‌ای برای شما ثبت نشده است.";
            }

            // --- جستجوی رزرو قبلی کاربر در این هفته ---
            var existingReserve = _foodReserveManager.GetAll()
                .FirstOrDefault(r => r.UserId == currentUserId &&
                                     r.WeekStartDate == weekStartDate &&
                                     r.WeekEndDate == weekEndDate);

            FoodReserve existingFoodReserve = null;
            List<FoodReserveDetail> existingReserveDetails = null;

            if (existingReserve != null)
            {
                existingFoodReserve = existingReserve;
                // فرض: متد یا خاصیتی برای گرفتن جزئیات رزرو بر اساس FoodReserveId دارید
                existingReserveDetails = _foodReserveDetailManager.GetAll()
                    .Where(d => d.FoodReserveId == existingReserve.Id) // فرض می‌کنیم FoodReserveDetail دارای FoodReserveId است
                    .ToList();
            }
            // --------------------------------------

            var allFoods = _foodManager.GetAll().Where(x => x.IsDeleted == false).ToList();
            var allFoodPlanDays = _foodPlanDayManager.GetAll().Where(x => x.IsDeleted == false);

            // ... (بقیه منطق ساخت دیکشنری‌های غذاها که قبلا داشتید) ...
            var mainFoodsDict = new Dictionary<(long, long), List<Foods>>();
            var dessertDict = new Dictionary<(long, long), List<Foods>>();
            var sideDishDict = new Dictionary<(long, long), List<Foods>>();

            var grouped = allFoodPlanDays.GroupBy(x => new { x.DayId, x.MealId });

            foreach (var group in grouped)
            {
                var key = (group.Key.DayId, group.Key.MealId);
                // ... (منطق پر کردن دیکشنری‌ها) ...
                mainFoodsDict[key] = group
                    .Where(x => x.Food != null && x.Food.FoodTypesId == 3)
                    .Select(x => x.Food)
                    .Distinct()
                    .ToList();

                dessertDict[key] = group
                    .Where(x => x.FoodDesserId.HasValue)
                    .Select(x => allFoods.FirstOrDefault(f => f.Id == x.FoodDesserId.Value))
                    .Where(x => x != null)
                    .Distinct()
                    .ToList();

                sideDishDict[key] = group
                    .Where(x => x.FoodDorchinId.HasValue)
                    .Select(x => allFoods.FirstOrDefault(f => f.Id == x.FoodDorchinId.Value))
                    .Where(x => x != null)
                    .Distinct()
                    .ToList();
            }

            var model = new FoodPlanReserveDTO
            {
                Days = _foodPlanDayManager.GetAllDay().Model,
                Meals = _mealManager.GetAll().ToList(),
                MainFoods = mainFoodsDict,
                Desserts = dessertDict,
                SideDishes = sideDishDict,

                // تخصیص مقادیر جدید
                ExistingFoodReserve = existingFoodReserve,
                ExistingReserveDetails = existingReserveDetails
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormCollection form)
        {
            var user = HttpContext.Session.GetUser();
            if (user == null)
                return RedirectToAction("IndexZP", "Authentication", new { area = "" });

            long userId = user.Id;

            try
            {
                var now = FoodTimingRules.GetIranNow();
                int diff = (7 + (now.DayOfWeek - DayOfWeek.Saturday)) % 7;
                DateTime weekStartDate = now.AddDays(-1 * diff).Date;
                DateTime weekEndDate = weekStartDate.AddDays(6).Date;

                var personalCode =
                    user.PersonCode?.ToString() ??
                    user.Username;

                var nationalCode =
                    user.NationalCode?.ToString();

                // وضعیت در هر درخواست ثبت دوباره از دیتابیس خوانده می‌شود.
                // با خاموش شدن رزرو آزاد، امکان دورزدن سهمیه از سمت کلاینت وجود ندارد.
                var freeReservationEnabled =
                    _constantManager.IsFreeFoodReservationEnabled();

                var reservableSlots = freeReservationEnabled
                    ? GetOpenReservableSlots(weekStartDate)
                    : _unitQuotaManager
                        .GetReservableSlots(
                            user.PersonId,
                            personalCode,
                            nationalCode,
                            user.OmdOrgId,
                            weekStartDate,
                            weekEndDate)
                        .ToHashSet();

                var timingPolicy = _constantManager.GetFoodTimingPolicy();
                var slotsBeforeTiming = reservableSlots.Count;
                reservableSlots.RemoveWhere(slot =>
                    !FoodTimingRules.CanReserve(
                        timingPolicy, slot.Date, now, out _));

                if (reservableSlots.Count == 0)
                {
                    TempData["Error"] =
                        timingPolicy.ReservationDeadlineEnabled && slotsBeforeTiming > 0
                            ? "مهلت رزرو روزهای مجاز این هفته پایان یافته است."
                            : freeReservationEnabled
                                ? "برنامه غذایی قابل رزرو یافت نشد."
                                : "سهمیه‌ای برای شما ثبت نشده است و امکان رزرو غذا ندارید.";
                    return RedirectToAction("Index");
                }

                // جستجوی رزرو قبلی برای هفته جاری
                //var existingReserve0 = _foodReserveManager.GetAll()
                //    .FirstOrDefault(r => r.UserId == userId &&
                //                        r.WeekStartDate == weekStartDate &&
                //                        r.WeekEndDate == weekEndDate);

                // ایجاد شی برنامه غذایی
                var plan = new FoodReserveDTO
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    WeekStartDate = weekStartDate,
                    WeekEndDate = weekEndDate,
                    Details = new List<FoodReserveDetailDTO>()
                };

                // گرفتن روزها و وعده‌ها
                var days = _foodPlanDayManager.GetAllDay().Model;
                var meals = _mealManager.GetAll().ToList();
                var plannedFoodItems = _foodPlanDayManager.GetAll()
                    .Where(x => x.IsDeleted == false)
                    .ToList();

                var unauthorizedSelection = false;
                var invalidFoodSelection = false;
                string cutoffViolationMessage = null;

                // فقط روز و وعده‌ای پذیرفته می‌شود که برای کد پرسنلی سهمیه داشته باشد.
                foreach (var day in days)
                {
                    var dayDate = weekStartDate.AddDays(day.Code - 1).Date;

                    foreach (var meal in meals)
                    {
                        var key = $"{day.Id}{meal.Id}";

                        var mainFood = form[$"MainFood_{key}"];
                        var dessert = form[$"Dessert_{key}"];
                        var sideDish = form[$"SideDish_{key}"];

                        if (string.IsNullOrEmpty(mainFood) &&
                            string.IsNullOrEmpty(dessert) &&
                            string.IsNullOrEmpty(sideDish))
                            continue;

                        if (!reservableSlots.Contains((dayDate, meal.Id)))
                        {
                            unauthorizedSelection = true;
                            if (!FoodTimingRules.CanReserve(
                                    timingPolicy, dayDate, now, out var cutoffReason))
                            {
                                cutoffViolationMessage ??= cutoffReason;
                            }
                            continue;
                        }

                        long.TryParse(mainFood, out var mainFoodId);
                        long.TryParse(dessert, out var dessertId);
                        long.TryParse(sideDish, out var sideDishId);

                        // شناسه غذاها باید از برنامه ثبت‌شده همین روز و وعده باشند.
                        // این کنترل در هر دو حالت آزاد و سهمیه‌ای الزامی است.
                        var dayId = Convert.ToInt64(day.Id);
                        var mealId = Convert.ToInt64(meal.Id);
                        var plannedForMeal = plannedFoodItems
                            .Where(x => x.DayId == dayId && x.MealId == mealId)
                            .ToList();

                        if (mainFoodId <= 0 ||
                            !plannedForMeal.Any(x => x.FoodId == mainFoodId) ||
                            (dessertId > 0 &&
                             !plannedForMeal.Any(x => x.FoodDesserId == dessertId)) ||
                            (sideDishId > 0 &&
                             !plannedForMeal.Any(x => x.FoodDorchinId == sideDishId)))
                        {
                            invalidFoodSelection = true;
                            continue;
                        }

                        plan.Details.Add(new FoodReserveDetailDTO
                        {
                            DayId = day.Id,
                            MealId = meal.Id,
                            MainFoodId = mainFoodId != 0 ? mainFoodId : (long?)null,
                            DessertId = dessertId != 0 ? dessertId : (long?)null,
                            SideDishId = sideDishId != 0 ? sideDishId : (long?)null
                        });
                    }
                }

                if (unauthorizedSelection)
                {
                    TempData["Error"] = cutoffViolationMessage ??
                        (freeReservationEnabled
                            ? "فقط روزها و وعده‌های موجود در برنامه غذایی قابل رزرو هستند."
                            : "امکان ثبت غذا برای روز یا وعده‌ای که سهمیه ندارید وجود ندارد.");
                    return RedirectToAction("Index");
                }

                if (invalidFoodSelection)
                {
                    TempData["Error"] =
                        "غذا، دسر یا دورچین انتخاب‌شده با برنامه غذایی این روز و وعده مطابقت ندارد.";
                    return RedirectToAction("Index");
                }

                // اگر هیچ جزئیتی وجود نداشت، خطا
                if (!plan.Details.Any())
                {
                    TempData["Error"] = "لطفاً حداقل یک مورد را انتخاب نمایید.";
                    return RedirectToAction("Index");
                }

                // فقط روز و وعده‌های واقعاً مجاز در این درخواست تغییر می‌کنند.
                // بقیه رکوردها (مهلت‌گذشته، بدون سهمیه یا برنامه قبلی)
                // همان شماره ژتون را حفظ می‌کنند.
                var dayCodes = days
                    .ToDictionary(x => x.Code, x => x.Id);

                var editableSlots = new HashSet<(long DayId, long MealId)>();
                foreach (var slot in reservableSlots)
                {
                    var dayCode = (int)(slot.Date.Date - weekStartDate).TotalDays + 1;
                    if (dayCodes.TryGetValue(dayCode, out var dayId))
                    {
                        editableSlots.Add((dayId, slot.MealId));
                    }
                }

                var result = _foodReserveManager.SaveWeeklyReservation(
                    plan, editableSlots);

                if (result.Status)
                {
                    TempData["Success"] = "برنامه غذایی با موفقیت ثبت شد.";

                    try
                    {
                        await _redis.db.RefreshUserSession(user.Id, 1, 3);
                    }
                    catch
                    {
                        // رزرو در DB ثبت شده و خطای تمدید Redis نباید
                        // به اشتباه به‌عنوان خطای ثبت غذا نمایش داده شود.
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "خطا در ثبت برنامه غذایی: " + result.Message;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "خطا در ثبت برنامه غذایی: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// روزها و وعده‌های دارای برنامه غذایی برای حالت رزرو آزاد.
        /// از تعریف برنامه غذا استفاده می‌شود؛ این حالت به معنای
        /// مجاز شدن وعده یا غذای تعریف‌نشده نیست.
        /// </summary>
        private HashSet<(DateTime Date, long MealId)> GetOpenReservableSlots(
            DateTime weekStartDate)
        {
            var days = _foodPlanDayManager.GetAllDay().Model;
            var plannedPairs = _foodPlanDayManager.GetAll()
                .Where(x => x.IsDeleted == false)
                .Select(x => (x.DayId, x.MealId))
                .ToHashSet();

            var mealIds = _mealManager.GetAll()
                .Select(x => x.Id)
                .ToHashSet();

            var slots = new HashSet<(DateTime Date, long MealId)>();
            foreach (var day in days)
            {
                var dayCode = Convert.ToInt32(day.Code);
                var dayId = Convert.ToInt64(day.Id);
                if (dayCode < 1 || dayCode > 7)
                    continue;

                var date = weekStartDate.AddDays(dayCode - 1).Date;
                foreach (var mealId in mealIds)
                {
                    if (plannedPairs.Contains((dayId, mealId)))
                        slots.Add((date, mealId));
                }
            }

            return slots;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDay(long dayId)
        {
            var user = HttpContext.Session.GetUser();
            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "نشست کاربری منقضی شده است؛ دوباره وارد سامانه شوید."
                });
            }

            long userId = user.Id;

            try
            {
                // محاسبه محدوده هفته جاری
                var now = FoodTimingRules.GetIranNow();
                int diff = (7 + (now.DayOfWeek - DayOfWeek.Saturday)) % 7;
                DateTime weekStartDate = now.AddDays(-1 * diff).Date;
                DateTime weekEndDate = weekStartDate.AddDays(6).Date;

                var foodDay = _foodPlanDayManager.GetAllDay().Model
                    .FirstOrDefault(x => x.Id == dayId);

                if (foodDay == null)
                {
                    return Json(new { success = false, message = "روز انتخاب‌شده معتبر نیست." });
                }

                var dayDate = weekStartDate.AddDays(foodDay.Code - 1).Date;
                var timingPolicy = _constantManager.GetFoodTimingPolicy();
                if (!FoodTimingRules.CanReserve(
                        timingPolicy, dayDate, now, out var timingMessage))
                {
                    return Json(new
                    {
                        success = false,
                        message = timingMessage ?? "مهلت تغییر رزرو این روز به پایان رسیده است."
                    });
                }

                // حذف در سرویس با یک Commit انجام می‌شود؛ حذف جزئیات
                // به‌صورت جداگانه نباید منجر به وضعیت نیمه‌کاره شود.
                var deleteResult = _foodReserveManager.DeleteDayReservation(
                    userId, weekStartDate, dayId);

                return Json(new
                {
                    success = deleteResult.Status,
                    message = deleteResult.Message
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    message = "حذف رزرو روز با خطا همراه بود."
                });
            }
        }







    }
}
