using Microsoft.AspNetCore.Http;

namespace Services.CookieServices
{
    /// <summary>
    /// مدیریت اطلاعات تبلیغات بسته شده درون کوکی
    /// </summary>
    public static class CookieAdManager
    {
        public static readonly string Key = "AdIds";

        /// <summary>
        /// گرفتن آیدی تبلیغ هایی که کاربر نمیخواهد آنها را ببیند
        /// </summary>
        /// <returns></returns>
        public static List<int> GetCookieAdIds(this IRequestCookieCollection RequestCookies)
        {
            try
            {
                var CookieIds = RequestCookies.Get(Key);
                if (CookieIds == null)
                    return new List<int>();
                var Ids = CookieIds.Split(',').Select(Int32.Parse).ToList();
                return Ids;
            }
            catch
            {
                return new List<int>();
            }
        }


        /// <summary>
        /// افزودن آیدی تبلیغ به کوکی
        /// </summary>
        /// <returns></returns>
        public static bool SetCookieAdIds(this HttpContext HttpContext, int AdId)
        {
            try
            {
                var RequestCookies = HttpContext.Request.Cookies;
                var ResponseCookies = HttpContext.Response.Cookies;

                var OldValue = RequestCookies.Get(Key);
                if (OldValue == null)
                    ResponseCookies.Set(Key, AdId.ToString());
                else
                {
                    var Ids = RequestCookies.GetCookieAdIds();
                    if (!Ids.Any(id => id == AdId))
                    {
                        Ids.Add(AdId);
                        var Value = string.Join(",", Ids);
                        ResponseCookies.Set(Key, Value);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// حذف آیدی تبلیغ از کوکی
        /// </summary>
        public static bool RemoveCookieAdIds(this HttpContext HttpContext)
        {
            try
            {
                HttpContext.RemoveCookie(Key);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
