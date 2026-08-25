using Microsoft.AspNetCore.Http;

namespace Services.CookieServices
{
    /// <summary>
    /// ست کردن توکن کاربر در کوکی برای چک کردن در هر 
    /// درخواست جهت جلوگیری از لاگین همزمان 2 نفر با یک اکانت
    /// </summary>
    public static class CookieUserTokenManager
    {
        public static readonly string Key = "UserToken";

        /// <summary>
        /// گرفتن توکن کاربر
        /// </summary>
        /// <returns></returns>
        public static string GetCookieUserToken(this HttpContext HttpContext)
        {
            try
            {
                return HttpContext?.Request?.Cookies?.Get(Key);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// افزودن توکن کاربر به کوکی
        /// </summary>
        /// <returns></returns>
        public static bool SetCookieUserToken(this HttpContext HttpContext, string Token)
        {
            try
            {
                var ResponseCookies = HttpContext?.Response?.Cookies;
                ResponseCookies?.Set(Key, Token, HttpOnly: true);                
                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// حذف توکن کاربر از کوکی
        /// </summary>
        public static bool RemoveCookieUserToken(this HttpContext HttpContext)
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
