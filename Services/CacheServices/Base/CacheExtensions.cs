using Microsoft.Extensions.Caching.Memory;
using System;

namespace Services.CacheServices
{
    /// <summary>
    /// ذخیره و بازیابی دیتا از کش
    /// </summary>
    public static class CacheExtensions
    {

        /// <summary>
        /// قرار دادن مقدار در کش
        /// </summary>
        /// <typeparam name="T">نوع داده ای که درون کش قرار میگیرد</typeparam>
        /// <param name="cache">کش</param>
        /// <param name="key">کلید</param>
        /// <param name="value">داده ای که درون کش قرار میگیرد</param>
        public static void Set<T>(this IMemoryCache  cache, string key, T value)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(CacheConfig.ExpireTime));
            cache.Set<T>(key, value, cacheOptions);
        }




        /// <summary>
        /// گرفتن مقدا از کش
        /// </summary>
        /// <typeparam name="T">نوع داده ای که درون کش قرار میگیرد</typeparam>
        /// <param name="cache">کش</param>
        /// <param name="key">کلید</param>
        /// <returns></returns>
        public static T Get<T>(this IMemoryCache  cache, string key)
        {
            T value;
            if (!cache.TryGetValue<T>(key, out value))
                return default(T);
            return value;
        }





        /// <summary>
        /// یک داده را از کش میخواند.
        /// در صورتی که داده مورد نظر در کش وجود نداشته باشد،
        /// از تابعی که در ورودی پاس داده میشود، مقدار مورد نظر را لود میکند و برمیگرداند.
        /// </summary>
        /// <typeparam name="T">نوع خروجی</typeparam>
        /// <param name="cache">کش</param>
        /// <param name="key">کلید</param>
        /// <param name="generator">تابعی که در صورت خالی بودن کش، آنرا پر میکند</param>
        /// <returns></returns>
        public static T GetOrStore<T>(this IMemoryCache  cache, string key, Func<T> generator)
        {
            T data;
            if (!cache.TryGetValue(key, out data))
            {
                data = generator != null ? generator() : default(T);
                return cache.GetOrStore(key, data);
            }
            return data;
        }





        /// <summary>
        /// یک داده را از کش میخواند.
        /// در صورتی که داده مورد نظر در کش وجود نداشته باشد،
        /// مقدار پاس داده شده در ورودی را درون کش قرار میدهد
        /// </summary>
        /// <typeparam name="T">نوع خروجی</typeparam>
        /// <param name="cache">کش</param>
        /// <param name="key">کلید</param>
        /// <param name="obj">داده ای که درون کش قرار میگیرد</param>
        /// <returns></returns>
        public static T GetOrStore<T>(this IMemoryCache  cache, string key, T obj)
        {
            T data;
            if (!cache.TryGetValue(key, out data))
            {
                data = obj != null ? obj : default(T);
                cache.Set<T>(key, data);                
            }
            return data;
        }




        /// <summary>
        /// پاک کردن کش
        /// </summary>
        public static void ClearCache(this IMemoryCache cache) {
            //cache.RemoveCategories();
            //cache.RemoveAboutUs();
            //cache.RemoveAds();
        }




    }
}
