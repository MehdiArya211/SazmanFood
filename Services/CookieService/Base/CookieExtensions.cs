using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.CookieServices
{
    /// <summary>
    /// خوندن و نوشتن از کوکی
    /// </summary>
    public static class CookieExtensions
    {
        private static object sync = new object();



        /// <summary>  
        /// set the cookie  
        /// </summary>  
        /// <param name="key">key (unique indentifier)</param>  
        /// <param name="value">value to store in cookie object</param>  
        /// <param name="expireTimeAsDay">expiration time</param>  
        public static void Set(this IResponseCookies ResponseCookies, string key, string value, int? expireTimeAsDay = 5, bool HttpOnly = false, DateTime? ExpDate = null)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = ExpDate ?? DateTime.Now.AddDays(expireTimeAsDay ?? 10).Date + new TimeSpan(23, 59, 59);
            option.HttpOnly = HttpOnly;
            option.IsEssential = true;
            option.SameSite = SameSiteMode.None;
            option.Secure = true;
            option.Path = "/";
            //option.Domain = ".etkala.ir";
            //option.Domain = Request.RequestUri.Host;
            ResponseCookies.Append(key, value, option);
        }



        /// <summary>
        /// قرار دادن مقدار در کوکی
        /// </summary>
        /// <typeparam name="T">نوع داده ای که درون کوکی قرار میگیرد</typeparam>
        /// <param name="ResponseCookies">ریسپانس کوکی</param>
        /// <param name="key">کلید</param>
        /// <param name="value">داده ای که درون کوکی قرار میگیرد</param>
        public static void Set<T>(this IResponseCookies ResponseCookies, string key, T value, int? expireTimeAsDay = 5, bool HttpOnly = false, DateTime? ExpDate = null)
        {
            var st = JsonConvert.SerializeObject(value);
            ResponseCookies.Set(key, st, expireTimeAsDay, HttpOnly, ExpDate);
        }




        /// <summary>
        /// گرفتن مقدا از کوکی
        /// </summary>
        /// <typeparam name="T">نوع داده ای که درون کوکی قرار میگیرد</typeparam>
        /// <param name="RequestCookies">ریکوئست کوکی</param>
        /// <param name="key">کلید</param>
        /// <returns></returns>
        public static T Get<T>(this IRequestCookieCollection RequestCookies, string key)
        {
            string value = RequestCookies.Get(key);
            if (string.IsNullOrEmpty(value))
                return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }



        /// <summary>
        /// گرفتن مقدا از کوکی
        /// </summary>
        /// <typeparam name="T">نوع داده ای که درون کوکی قرار میگیرد</typeparam>
        /// <param name="Request">ریکوئست</param>
        /// <param name="key">کلید</param>
        /// <returns></returns>
        public static string Get(this IRequestCookieCollection RequestCookies, string key)
        {
            string value = RequestCookies[key];
            return value;
        }




        /// <summary>
        /// یک داده را از کوکی میخواند.
        /// در صورتی که داده مورد نظر در کوکی وجود نداشته باشد،
        /// از تابعی که در ورودی پاس داده میشود، مقدار مورد نظر را لود میکند و برمیگرداند.
        /// </summary>
        /// <typeparam name="T">نوع خروجی</typeparam>
        /// <param name="HttpContext">کانتکست</param>
        /// <param name="key">کلید</param>
        /// <param name="generator">تابعی که در صورت خالی بودن کوکی، آنرا پر میکند</param>
        /// <returns></returns>
        public static T GetOrStoreCookie<T>(this HttpContext HttpContext, string key, Func<T> generator, bool HttpOnly = false)
        {
            var value = HttpContext.Request.Cookies.Get<T>(key);
            return HttpContext.GetOrStoreCookie(key, (value == null && generator != null) ? generator() : default(T), HttpOnly);
        }





        /// <summary>
        /// یک داده را از کوکی میخواند.
        /// در صورتی که داده مورد نظر در کوکی وجود نداشته باشد،
        /// مقدار پاس داده شده در ورودی را درون کوکی قرار میدهد
        /// </summary>
        /// <typeparam name="T">نوع خروجی</typeparam>
        /// <param name="HttpContext">کانتکست</param>
        /// <param name="key">کلید</param>
        /// <param name="obj">داده ای که درون کوکی قرار میگیرد</param>
        /// <returns></returns>
        public static T GetOrStoreCookie<T>(this HttpContext HttpContext, string key, T obj, bool HttpOnly = false)
        {
            var RequestCookies = HttpContext.Request.Cookies;
            var ResponseCookies = HttpContext.Response.Cookies;
            T result = RequestCookies.Get<T>(key);

            if (result == null)
            {
                lock (sync)
                {
                    result = RequestCookies.Get<T>(key);
                    if (result == null)
                    {
                        result = obj != null ? obj : default(T);
                        ResponseCookies.Set<T>(key, result, HttpOnly: HttpOnly);
                    }
                }
            }
            return result;
        }




        public static void RemoveCookie(this HttpContext HttpContext, string Key)
        {
            var RequestCookies = HttpContext.Request.Cookies;
            var ResponseCookies = HttpContext.Response.Cookies;
            if (RequestCookies.Get(Key) != null)
                ResponseCookies.Delete(Key);
        }

    }
}








