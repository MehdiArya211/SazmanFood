using BLL;
using BLL.ReserveManagment;
using BLL.Interface;
using DTO.User;
using DTO.Entities.FoodReservation;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.RedisService;
using Services.SessionServices;

namespace Food.Areas.ShowReserveManagment.Controllers
{
    /// <summary>
    /// مدیریت رزرو غذا - غذا
    /// </summary>
    [Area("ShowReserveManagment")]
    [UserAuthorize(IsPublic: true)]
    public class ShowReserveController : Controller
    {
        private readonly IFoodReserveManager _foodReserveManager;
        private readonly IRedisManager _redis;
        private readonly ILogger<ShowReserveController> _logger;
        private readonly IConstantManager _constantManager;
        private readonly IUnitQuotaManager _unitQuotaManager;


        public ShowReserveController(
            IFoodReserveManager foodReserveManager,
            IRedisManager redis,
            ILogger<ShowReserveController> logger,
            IConstantManager constantManager,
            IUnitQuotaManager unitQuotaManager)
        {
            _foodReserveManager = foodReserveManager;
            _redis = redis;
            _logger = logger;
            _constantManager = constantManager;
            _unitQuotaManager = unitQuotaManager;
        }


        /// <summary>
        /// در حالت سهمیه‌ای، برای هر ژتون تاریخ و شناسه وعده کنترل می‌شود.
        /// این کنترل هم برای چاپ سرور و هم برای چاپ مرورگر استفاده می‌شود.
        /// </summary>
        private HashSet<long> GetPrintableIds(
            UserSessionDTO user,
            IReadOnlyCollection<WeeklyFoodReserveDTO> reservations)
        {
            if (reservations == null || reservations.Count == 0)
                return new HashSet<long>();

            var timingPolicy = _constantManager.GetFoodTimingPolicy();
            var nowIran = FoodTimingRules.GetIranNow();

            // سیاست زمانی همیشه مستقل از آزاد یا سهمیه‌ای بودن غذا اعمال می‌شود.
            var timeEligible = reservations
                .Where(x => FoodTimingRules.CanPrint(
                    timingPolicy, x.Date, nowIran, out _))
                .ToList();

            if (timeEligible.Count == 0)
                return new HashSet<long>();

            if (_constantManager.IsFreeFoodReservationEnabled())
                return timeEligible
                    .Select(x => x.FoodReserveDetaileId)
                    .ToHashSet();

            var first = timeEligible.Min(x => x.Date).Date;
            var last = timeEligible.Max(x => x.Date).Date;
            var personCode = user.PersonCode?.ToString() ?? user.Username;
            var nationalCode = user.NationalCode?.ToString();
            var allowed = _unitQuotaManager.GetReservableSlots(
                user.PersonId, personCode, nationalCode, user.OmdOrgId,
                first, last).ToHashSet();

            return timeEligible
                .Where(x => allowed.Contains((x.Date.Date, x.MealId)))
                .Select(x => x.FoodReserveDetaileId)
                .ToHashSet();
        }

        public async Task<IActionResult> Index0()
        {
            var user = HttpContext.Session.GetUser();
            //await _foodReserveManager.PrintFoodReserveAsync(user.Id);

            if (user == null)
                return RedirectToAction("Login", "FaceAuth");

            var weeklyMeals = await _foodReserveManager.GetWeeklyFoodReserve(user.Id);
            return View(weeklyMeals);
        }

        public async Task<IActionResult> Index()
        {
            var user = HttpContext.Session.GetUser();
            if (user == null)
                return RedirectToAction("Login", "FaceAuth");

            var weeklyMeals = await _foodReserveManager.GetWeeklyFoodReserve(user.Id);
            ViewBag.AllowedPrintIds = GetPrintableIds(user, weeklyMeals);
            var timingPolicy = _constantManager.GetFoodTimingPolicy();
            ViewBag.PrintRestrictionEnabled = timingPolicy.PrintTimeEnabled;
            ViewBag.PrintStartTime = timingPolicy.PrintStartTime;
            ViewBag.IranToday = FoodTimingRules.GetIranNow().ToString("yyyy-MM-dd");

            // نبودن Redis نباید دسترسی کاربر به رسید رزرو و چاپ را قطع کند.
            try
            {
                await _redis.db.RefreshUserSession(user.Id, /*DeviceId*/ 1, 3);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "تمدید نشست Redis هنگام نمایش ژتون ناموفق بود. UserId: {UserId}",
                    user.Id);
            }

            return View(weeklyMeals);
        }


        /// <summary>
        /// رسیدهای رزرو متعلق به کاربر جاری را برای چاپ در مرورگر آماده می‌کند.
        /// از این مسیر، چاپگر رایانه کاربر (نه چاپگر نصب‌شده روی سرور) استفاده می‌شود.
        /// </summary>
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> PrintPreview(string foodReserveDetaileIds)
        {
            var user = HttpContext.Session.GetUser();
            if (user == null)
                return Unauthorized("نشست کاربری منقضی شده است.");

            var ids = (foodReserveDetaileIds ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(value => long.TryParse(value, out var id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToHashSet();

            if (ids.Count == 0 || ids.Count > 30)
                return BadRequest("شناسه ژتون‌های انتخاب‌شده معتبر نیست.");

            // داده‌ها فقط از رزروهای همین کاربر خوانده می‌شوند؛
            // شناسه‌های ارسالی مرورگر به تنهایی مجوز چاپ ایجاد نمی‌کنند.
            var userReservations =
                await _foodReserveManager.GetWeeklyFoodReserve(user.Id);

            var printableItems = userReservations
                .Where(item => ids.Contains(item.FoodReserveDetaileId))
                .OrderBy(item => item.Date)
                .ThenBy(item => item.MealTitle)
                .ToList();

            if (printableItems.Count == 0 ||
                printableItems.Count != ids.Count)
            {
                _logger.LogWarning(
                    "درخواست چاپ مرورگری با شناسه نامعتبر یا متعلق به کاربر دیگر. UserId: {UserId}, Ids: {Ids}",
                    user.Id,
                    string.Join(",", ids));
                return NotFound("ژتون‌های انتخاب‌شده برای این کاربر یافت نشد.");
            }

            var permittedIds = GetPrintableIds(user, printableItems);
            if (!ids.IsSubsetOf(permittedIds))
            {
                var policy = _constantManager.GetFoodTimingPolicy();
                var nowIran = FoodTimingRules.GetIranNow();
                var timingError = printableItems
                    .Select(item =>
                    {
                        FoodTimingRules.CanPrint(policy, item.Date, nowIran, out var message);
                        return message;
                    })
                    .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message));

                _logger.LogWarning(
                    "چاپ مرورگری بدون سهمیه یا خارج از ساعت مجاز رد شد. UserId: {UserId}, Ids: {Ids}",
                    user.Id, string.Join(",", ids));
                return StatusCode(StatusCodes.Status403Forbidden,
                    timingError ?? "برای چاپ این ژتون سهمیه فعال ندارید.");
            }

            ViewBag.PrintUserName = user.FullName;
            _logger.LogInformation(
                "پیش‌نمایش چاپ ژتون در مرورگر ایجاد شد. UserId: {UserId}, DetailIds: {DetailIds}",
                user.Id,
                string.Join(",", ids));

            return View(printableItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrintMealMaxa(
            string foodReserveDetaileIds)
        {
            return await PrintMeal(
                foodReserveDetaileIds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrintMeal(
            string foodReserveDetaileIds)
        {
            var user =
                HttpContext.Session.GetUser();

            if (user == null)
            {
                return Unauthorized(new
                {
                    Status = false,
                    Message =
                        "نشست کاربری منقضی شده است."
                });
            }

            var ids =
                (foodReserveDetaileIds ??
                 string.Empty)
                    .Split(
                        ',',
                        StringSplitOptions
                            .RemoveEmptyEntries)
                    .Select(x =>
                        long.TryParse(
                            x,
                            out var id)
                            ? id
                            : 0)
                    .Where(x => x > 0)
                    .Distinct()
                    .ToList();

            if (!ids.Any())
            {
                _logger.LogWarning(
                    "درخواست چاپ ژتون بدون شناسه معتبر ارسال شد. UserId: {UserId}",
                    user.Id);

                return BadRequest(new
                {
                    Status = false,
                    Message =
                        "شناسه ژتون برای چاپ معتبر نیست."
                });
            }

            try
            {
                var reservations =
                    await _foodReserveManager.GetWeeklyFoodReserve(user.Id);
                var owned = reservations
                    .Where(x => ids.Contains(x.FoodReserveDetaileId))
                    .ToList();

                var permittedIds = GetPrintableIds(user, owned);
                if (owned.Count != ids.Count ||
                    !ids.All(permittedIds.Contains))
                {
                    var policy = _constantManager.GetFoodTimingPolicy();
                    var nowIran = FoodTimingRules.GetIranNow();
                    var timingError = owned
                        .Select(item =>
                        {
                            FoodTimingRules.CanPrint(policy, item.Date, nowIran, out var message);
                            return message;
                        })
                        .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message));

                    _logger.LogWarning(
                        "چاپ سرور بدون سهمیه یا خارج از ساعت مجاز رد شد. UserId: {UserId}, Ids: {Ids}",
                        user.Id, string.Join(",", ids));
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new
                        {
                            Status = false,
                            Message = timingError ??
                                "رزرو یا سهمیه معتبر برای چاپ ژتون انتخاب‌شده یافت نشد."
                        });
                }

                var result =
                    await _foodReserveManager
                        .PrintFoodReserveAsync(
                            ids,
                            user.Id,
                            user.FullName);

                if (result.Status)
                {
                    _logger.LogInformation(
                        "درخواست ژتون به صف چاپ سرور ارسال شد. UserId: {UserId}, DetailIds: {DetailIds}, Message: {Message}",
                        user.Id,
                        string.Join(",", ids),
                        result.Message);
                }
                else
                {
                    _logger.LogWarning(
                        "چاپ ژتون غذا ناموفق بود. UserId: {UserId}, DetailIds: {DetailIds}, Message: {Message}",
                        user.Id,
                        string.Join(",", ids),
                        result.Message);
                }

                if (result.Status)
                {
                    try
                    {
                        await _redis.db.RefreshUserSession(user.Id, 1, 3);
                    }
                    catch (Exception refreshException)
                    {
                        // خطای تمدید نشست، وضعیت ارسال چاپ را تغییر نمی‌دهد.
                        _logger.LogWarning(
                            refreshException,
                            "تمدید نشست پس از ارسال چاپ ناموفق بود. UserId: {UserId}",
                            user.Id);
                    }
                }

                return Json(new
                {
                    Status = result.Status,
                    Message = result.Message,
                    Model = result.Model
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "خطای پیش‌بینی‌نشده هنگام چاپ ژتون غذا. UserId: {UserId}, DetailIds: {DetailIds}",
                    user.Id,
                    string.Join(",", ids));

                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    new
                    {
                        Status = false,
                        Message =
                            "چاپ ژتون با خطای پیش‌بینی‌نشده همراه بود."
                    });
            }
        }


    }
}
