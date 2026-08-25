using Microsoft.AspNetCore.Http;

namespace Services.CookieServices
{
    /// <summary>
    /// ست کردن کوکی برای نمایش کپچا
    /// </summary>
    public static class CookieShowCaptchaManager
    {
        public static readonly string Key = "shwcpt";

        /// <summary>
        /// گرفتن اطلاعات نمایش کپچا
        /// </summary>
        /// <returns></returns>
        public static bool GetCookieShowCaptcha(this HttpContext HttpContext)
        {
            try
            {
                return HttpContext?.Request?.Cookies?.Get(Key) == "true";
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// افزودن اطلاعات نمایش کپچا به کوکی
        /// </summary>
        /// <returns></returns>
        public static bool SetCookieShowCaptcha(this HttpContext HttpContext)
        {
            try
            {
                var ResponseCookies = HttpContext?.Response?.Cookies;
                ResponseCookies?.Set(Key, "true", HttpOnly: true, ExpDate: DateTime.Now.AddMinutes(60));                
                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// حذف اطلاعات نمایش کپچا از کوکی
        /// </summary>
        public static bool RemoveCookieShowCaptcha(this HttpContext HttpContext)
        {
            try
            {
                HttpContext?.RemoveCookie(Key);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
