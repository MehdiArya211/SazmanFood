using Microsoft.AspNetCore.Mvc;

namespace Food.Controllers
{
    /// <summary>
    /// مسیرهای قدیمی ورود با چهره برای سازگاری لینک‌های قبلی حفظ شده‌اند.
    /// ورود با userId یا correlationId داخل URL دیگر مجاز نیست.
    /// </summary>
    public class FaceAuthController : Controller
    {
        [HttpGet]
        public IActionResult LoginByFace(string userId, string correlationId)
        {
            return RedirectToAction("IndexZP", "Authentication", new { area = "" });
        }

        [HttpGet]
        public IActionResult LoginByFace1(string userId, string correlationId)
        {
            return RedirectToAction("IndexZP", "Authentication", new { area = "" });
        }
    }
}
