using BLL;
using BLL.ReserveManagment;
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


        public ShowReserveController(
            IFoodReserveManager foodReserveManager,
            IRedisManager redis,
            ILogger<ShowReserveController> logger)
        {
            _foodReserveManager = foodReserveManager;
            _redis = redis;
            _logger = logger;
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

            // 🟢 تمدید سشن کاربر
            await _redis.db.RefreshUserSession(user.Id, /*DeviceId*/ 1, 3);

            return View(weeklyMeals);
        }


        [HttpGet]
        public async Task<IActionResult> PrintMealMaxa(
            string foodReserveDetaileIds)
        {
            return await PrintMeal(
                foodReserveDetaileIds);
        }

        [HttpGet]
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
                var result =
                    await _foodReserveManager
                        .PrintFoodReserveAsync(
                            ids,
                            user.Id,
                            user.FullName);

                if (result.Status)
                {
                    _logger.LogInformation(
                        "چاپ ژتون غذا موفق بود. UserId: {UserId}, DetailIds: {DetailIds}, Message: {Message}",
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
                    await _redis.db
                        .RefreshUserSession(
                            user.Id,
                            1,
                            3);
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
