using BLL.FoodManag.KitchenBL;
using Domain.Constants;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.SessionServices;

namespace Food.Areas.FoodPlan.Controllers
{
    /// <summary>
    /// گزارش آمار پخت آشپزخانه‌ها
    /// </summary>
    [Area("FoodPlan")]
    [UserAuthorize(
        Area: "FoodPlan",
        Controller: "KitchenCookingStatistics",
        Action: "Index")]
    public class KitchenCookingStatisticsController : Controller
    {
        private readonly IKitchensManager kitchensManager;
        private readonly ISession Session;

        public KitchenCookingStatisticsController(
            IKitchensManager _kitchensManager,
            IHttpContextAccessor httpContextAccessor)
        {
            kitchensManager = _kitchensManager;
            Session = httpContextAccessor.HttpContext.Session;
        }

        #region نمایش گزارش

        [HttpGet]
        public IActionResult Index(
            DateTime? fromDate,
            DateTime? toDate,
            long? kitchenId,
            long? mealId)
        {
            var user = Session.GetUser();

            if (user?.IsEnabled != true ||
                user.RoleId != RoleConstant.Admin)
            {
                return Forbid();
            }

            var from = (fromDate ?? DateTime.Today).Date;
            var to = (toDate ?? from.AddDays(6)).Date;

            if (to < from)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            if ((to - from).TotalDays > 92)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "بازه گزارش نمی‌تواند بیشتر از ۹۳ روز باشد.");

                to = from.AddDays(92);
            }

            var model = kitchensManager.GetCookingStatistics(
                from,
                to,
                kitchenId,
                mealId);

            return View(model);
        }

        #endregion
    }
}
