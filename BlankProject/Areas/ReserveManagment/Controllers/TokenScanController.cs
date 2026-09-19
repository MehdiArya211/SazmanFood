using BLL;
using Microsoft.AspNetCore.Mvc;

namespace Food.Areas.ReserveManagment.Controllers
{
    /// <summary>
    /// اعتبارسنجی و مصرف یک‌بارمصرف QR ژتون غذا.
    /// این مسیر برای باز شدن مستقیم توسط دوربین یا دستگاه اسکنر عمومی است.
    /// </summary>
    [Area("ReserveManagment")]
    public class TokenScanController : Controller
    {
        private readonly IQoutaPersonManager _qoutaPersonManager;
        private readonly ILogger<TokenScanController> _logger;

        public TokenScanController(
            IQoutaPersonManager qoutaPersonManager,
            ILogger<TokenScanController> logger)
        {
            _qoutaPersonManager = qoutaPersonManager;
            _logger = logger;
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Use(string code, string key, long mealId)
        {
            var result = _qoutaPersonManager.DeliverFoodByQr(
                code,
                key,
                mealId);

            if (result.Status)
            {
                _logger.LogInformation(
                    "QR ژتون غذا با موفقیت مصرف شد. DeliveryCode: {DeliveryCode}، MealId: {MealId}",
                    code,
                    mealId);
            }
            else
            {
                _logger.LogWarning(
                    "اسکن QR ژتون غذا ناموفق بود. DeliveryCode: {DeliveryCode}، MealId: {MealId}، Message: {Message}",
                    code,
                    mealId,
                    result.Message);
            }

            return View(result);
        }
    }
}
