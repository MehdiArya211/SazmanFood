using BLL;
using BLL.FoodManag.FoodBL;
using BLL.Interface;
using BLL.ReserveManagment;
using Domain.Constants;
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
    [UserAuthorize(Area: "ReserveManagment", Controller: "reserve", Action: "index")]
    public class ReserveController : Controller
    {
        private readonly IFoodManager _foodManager;
        private readonly IFoodPlanDayManager _foodPlanDayManager;
        private readonly IMealManager _mealManager;
        private readonly IFoodReserveManager _foodReserveManager;
        private readonly IFoodReserveDetailManager _foodReserveDetailManager;
        private readonly IRedisManager _redis;
        private readonly IUnitQuotaManager _unitQuotaManager;

        public ReserveController(IFoodPlanDayManager foodPlanDayManager, IMealManager mealManager, IFoodManager foodManager
            , IFoodReserveManager foodReserveManager, IRedisManager redis,
            IFoodReserveDetailManager foodReserveDetailManager,
            IUnitQuotaManager unitQuotaManager)
        {
            _foodPlanDayManager = foodPlanDayManager;
            _foodManager = foodManager;
            _mealManager = mealManager;
            _foodReserveManager = foodReserveManager;
            _redis = redis;
            _foodReserveDetailManager = foodReserveDetailManager;
            _unitQuotaManager = unitQuotaManager;
        }

        public IActionResult Index()
        {
            var user = HttpContext.Session.GetUser();
            if (user == null)
                return RedirectToAction("IndexZP", "Authentication", new { area = "" });

            if (user.RoleId != RoleConstant.MealBooker)
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            long currentUserId = user.Id;

            // --- محاسبه محدوده زمانی هفته جاری ---
            var now = DateTime.Now;
            // محاسبه شروع هفته (فرض: شروع هفته از شنبه)
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Saturday)) % 7;
            DateTime weekStartDate = now.AddDays(-1 * diff).Date;
            DateTime weekEndDate = weekStartDate.AddDays(6).Date;

            var personalCode =
                user.PersonCode?.ToString() ??
                user.Username;

            var nationalCode =
                user.NationalCode?.ToString();

            var reservableSlots = _unitQuotaManager
                .GetReservableSlots(
                    user.PersonId,
                    personalCode,
                    nationalCode,
                    user.OmdOrgId,
                    weekStartDate,
                    weekEndDate)
                .ToHashSet();

            ViewBag.WeekStartDate = weekStartDate;
            ViewBag.AllowedReservationSlots = reservableSlots
                .Select(x => $"{x.Date:yyyyMMdd}_{x.MealId}")
                .ToHashSet();
            ViewBag.HasReservationQuota = reservableSlots.Count > 0;

            if (reservableSlots.Count == 0)
            {
                ViewBag.QuotaError = "سهمیه‌ای برای شما ثبت نشده است.";
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
        public IActionResult Index(IFormCollection form)
        {
            var user = HttpContext.Session.GetUser();
            if (user == null)
                return RedirectToAction("IndexZP", "Authentication", new { area = "" });

            if (user.RoleId != RoleConstant.MealBooker)
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            long userId = user.Id;

            try
            {
                var now = DateTime.Now;
                int diff = (7 + (now.DayOfWeek - DayOfWeek.Saturday)) % 7;
                DateTime weekStartDate = now.AddDays(-1 * diff).Date;
                DateTime weekEndDate = weekStartDate.AddDays(6).Date;

                var personalCode =
                    user.PersonCode?.ToString() ??
                    user.Username;

                var nationalCode =
                    user.NationalCode?.ToString();

                var reservableSlots = _unitQuotaManager
                    .GetReservableSlots(
                        user.PersonId,
                        personalCode,
                        nationalCode,
                        user.OmdOrgId,
                        weekStartDate,
                        weekEndDate)
                    .ToHashSet();

                if (reservableSlots.Count == 0)
                {
                    TempData["Error"] = "سهمیه‌ای برای شما ثبت نشده است و امکان رزرو غذا ندارید.";
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

                var unauthorizedSelection = false;

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
                            continue;
                        }

                        long.TryParse(mainFood, out var mainFoodId);
                        long.TryParse(dessert, out var dessertId);
                        long.TryParse(sideDish, out var sideDishId);

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
                    TempData["Error"] =
                        "امکان ثبت غذا برای روز یا وعده‌ای که سهمیه ندارید وجود ندارد.";
                    return RedirectToAction("Index");
                }

                // اگر هیچ جزئیتی وجود نداشت، خطا
                if (!plan.Details.Any())
                {
                    TempData["Error"] = "لطفاً حداقل یک مورد را انتخاب نمایید.";
                    return RedirectToAction("Index");
                }

                // اعتبارسنجی کامل انجام شد؛ اکنون رزرو قبلی هفته جایگزین می‌شود.
                var existingReserveDetail = _foodReserveManager.GetFoodReserveDetail(userId);
                if (existingReserveDetail.Count != 0)
                {
                    var existingReserve = _foodReserveManager.GetById(
                        existingReserveDetail.First().FoodReserveId);

                    foreach (var item in existingReserveDetail)
                    {
                        _foodReserveDetailManager.Delete(item.Id);
                    }

                    if (existingReserve != null)
                    {
                        _foodReserveManager.Delete(existingReserve.Id);
                    }
                }

                var result = _foodReserveManager.AddToReserveAndReserveDetail(plan);

                if (result.Status)
                {
                    TempData["Success"] = "برنامه غذایی با موفقیت ثبت شد.";
                    _redis.db.RefreshUserSession(user.Id, 1, 3);
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

        [HttpPost]
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

            if (user.RoleId != RoleConstant.MealBooker)
            {
                return Json(new
                {
                    success = false,
                    message = "دسترسی به رزرو غذا فقط برای نقش رزروکننده مجاز است."
                });
            }

            long userId = user.Id;

            try
            {
                // محاسبه محدوده هفته جاری
                var now = DateTime.Now;
                int diff = (7 + (now.DayOfWeek - DayOfWeek.Saturday)) % 7;
                DateTime weekStartDate = now.AddDays(-1 * diff).Date;
                DateTime weekEndDate = weekStartDate.AddDays(6).Date;

                // جستجوی رزرو فعلی
                var existingReserve = _foodReserveManager.GetAll()
                    .FirstOrDefault(r => r.UserId == userId &&
                                        r.WeekStartDate == weekStartDate &&
                                        r.WeekEndDate == weekEndDate);

                if (existingReserve == null)
                {
                    return Json(new { success = false, message = "رزرو ایجاد نشده است." });
                }

                // حذف تمام جزئیات مربوط به این روز
                var detailsToDelete = _foodReserveDetailManager.GetAll()
                    .Where(d => d.FoodReserveId == existingReserve.Id && d.DayId == dayId)
                    .ToList();

                foreach (var detail in detailsToDelete)
                {
                    _foodReserveDetailManager.Delete(detail.Id);
                }

                // اگر هیچ جزئیاتی برای این رزرو باقی نماند، رزرو را حذف کن
                var remainingDetails = _foodReserveDetailManager.GetAll()
                    .Where(d => d.FoodReserveId == existingReserve.Id)
                    .ToList();

                if (!remainingDetails.Any())
                {
                    _foodReserveManager.Delete(existingReserve.Id);
                }

                return Json(new { success = true, message = "روز با موفقیت حذف شد." });
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
