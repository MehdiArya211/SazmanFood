using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Food.Controllers
{
    /// <summary>
    /// تولید کد امنیتی به‌صورت SVG و بدون وابستگی به فونت یا System.Drawing.
    /// </summary>
    public class CaptchaController : Controller
    {
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult CaptchaImage()
        {
            var captcha = RandomNumberGenerator.GetInt32(10000, 100000).ToString();

            HttpContext.Session.SetString("Captcha", captcha);

            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            var svg = $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""200"" height=""70"" viewBox=""0 0 200 70"">
  <rect width=""200"" height=""70"" rx=""8"" fill=""#f4f6f8""/>
  <path d=""M5 18 C45 2, 75 45, 195 15 M8 55 C65 28, 115 70, 192 42"" fill=""none"" stroke=""#b8c2cc"" stroke-width=""2""/>
  <line x1=""15"" y1=""12"" x2=""180"" y2=""60"" stroke=""#d1d8df"" stroke-width=""1""/>
  <line x1=""18"" y1=""60"" x2=""188"" y2=""10"" stroke=""#d1d8df"" stroke-width=""1""/>
  <text x=""100"" y=""48"" text-anchor=""middle"" font-family=""Tahoma, Arial, sans-serif""
        font-size=""34"" font-weight=""700"" letter-spacing=""8"" fill=""#25364a"">{captcha}</text>
</svg>";

            return Content(svg, "image/svg+xml", Encoding.UTF8);
        }
    }
}
