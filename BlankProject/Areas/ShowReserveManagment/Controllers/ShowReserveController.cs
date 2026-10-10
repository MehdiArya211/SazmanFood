using BLL;
using BLL.ReserveManagment;
using Domain.Constants;
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
    [UserAuthorize(Area: "ShowReserveManagment", Controller: "ShowReserve", Action: "index")]
    public class ShowReserveController : Controller
    {
        private readonly IFoodReserveManager _foodReserveManager;
        private readonly IRedisManager _redis;


        public ShowReserveController( IFoodReserveManager foodReserveManager,
            IRedisManager redis)
        {
            _foodReserveManager = foodReserveManager;
            _redis = redis;
        }


        public async Task<IActionResult> Index0()
        {
            var user = HttpContext.Session.GetUser();
            //await _foodReserveManager.PrintFoodReserveAsync(user.Id);

            if (user == null)
                return RedirectToAction("Login", "FaceAuth");

            if (user.RoleId != RoleConstant.MealBooker)
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            var weeklyMeals = await _foodReserveManager.GetWeeklyFoodReserve(user.Id);
            return View(weeklyMeals);
        }

        public async Task<IActionResult> Index()
        {
            var user = HttpContext.Session.GetUser();
            if (user == null)
                return RedirectToAction("Login", "FaceAuth");

            if (user.RoleId != RoleConstant.MealBooker)
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            var weeklyMeals = await _foodReserveManager.GetWeeklyFoodReserve(user.Id);

            // 🟢 تمدید سشن کاربر
            await _redis.db.RefreshUserSession(user.Id, /*DeviceId*/ 1, 3);

            return View(weeklyMeals);
        }


        [HttpGet]
        public async Task<IActionResult> PrintMealMaxa(string foodReserveDetaileIds)
        {
            var ids = foodReserveDetaileIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(idStr =>
                {
                    bool parsed = long.TryParse(idStr, out long id);
                    return new { parsed, id };
                })
                .Where(x => x.parsed)
                .Select(x => x.id)
                .ToList();

            var user = HttpContext.Session.GetUser();
            if (user == null)
                return Unauthorized();

            if (user.RoleId != RoleConstant.MealBooker)
                return Forbid();

            if (user.RoleId != RoleConstant.MealBooker)
                return Forbid();

            await _foodReserveManager.PrintFoodReserveAsync(ids, user.Id, user.FullName);

            // 🟢 تمدید سشن بعد از چاپ ژتون
            await _redis.db.RefreshUserSession(user.Id, /*DeviceId*/ 1, 3);

            TempData["Message"] = "✅ سفارش چاپ فاکتور ارسال شد!";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> PrintMeal(string foodReserveDetaileIds)
        {
            var ids = foodReserveDetaileIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(idStr => long.TryParse(idStr, out var id) ? id : 0)
                .Where(id => id > 0)
                .ToList();

            var user = HttpContext.Session.GetUser();
            if (user == null)
                return Unauthorized();


            await _foodReserveManager.PrintFoodReserveAsync(ids, user.Id, user.FullName);

            await _redis.db.RefreshUserSession(user.Id, 1, 3);

            TempData["Message"] = "✅ سفارش چاپ فاکتور ارسال شد!";
            return RedirectToAction("Index");
        }


    }
}
