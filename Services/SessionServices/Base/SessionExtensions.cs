using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.SessionServices
{
    /// <summary>
    /// خوندن و نوشتن از سشن
    /// </summary>
    public static class SessionExtensions
    {
        private static object sync = new object();






        /// <summary>
        /// قرار دادن مقدار در سشن
        /// </summary>
        /// <typeparam name="T">نوع داده ای که درون سشن قرار میگیرد</typeparam>
        /// <param name="session">سشن</param>
        /// <param name="key">کلید</param>
        /// <param name="value">داده ای که درون سشن قرار میگیرد</param>
        public static void Set<T>(this ISession session, string key, T value)
        {
            var st = JsonConvert.SerializeObject(value);
            session.Set(key, Encoding.UTF8.GetBytes(st));
        }




        /// <summary>
        /// گرفتن مقدا از سشن
        /// </summary>
        /// <typeparam name="T">نوع داده ای که درون سشن قرار میگیرد</typeparam>
        /// <param name="session">سشن</param>
        /// <param name="key">کلید</param>
        /// <returns></returns>
        public static T Get<T>(this ISession session, string key)
        {
            byte[] data;
            if(!session.TryGetValue(key, out data))
                return default(T);

            string value = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(value);
        }





        /// <summary>
        /// یک داده را از سشن میخواند.
        /// در صورتی که داده مورد نظر در سشن وجود نداشته باشد،
        /// از تابعی که در ورودی پاس داده میشود، مقدار مورد نظر را لود میکند و برمیگرداند.
        /// </summary>
        /// <typeparam name="T">نوع خروجی</typeparam>
        /// <param name="Session">سشن</param>
        /// <param name="key">کلید</param>
        /// <param name="generator">تابعی که در صورت خالی بودن سشن، آنرا پر میکند</param>
        /// <returns></returns>
        public static T GetOrStore<T>(this ISession Session, string key, Func<T> generator)
        {
            var value = Session.Get<T>(key);
            return Session.GetOrStore(key, (value == null && generator != null) ? generator() : default(T));
        }





        /// <summary>
        /// یک داده را از سشن میخواند.
        /// در صورتی که داده مورد نظر در سشن وجود نداشته باشد،
        /// مقدار پاس داده شده در ورودی را درون سشن قرار میدهد
        /// </summary>
        /// <typeparam name="T">نوع خروجی</typeparam>
        /// <param name="Session">سشن</param>
        /// <param name="key">کلید</param>
        /// <param name="obj">داده ای که درون سشن قرار میگیرد</param>
        /// <returns></returns>
        public static T GetOrStore<T>(this ISession Session, string key, T obj)
        {
            T result = Session.Get<T>(key);

            if (result == null)
            {
                lock (sync)
                {
                    result = Session.Get<T>(key);
                    if (result == null)
                    {
                        result = obj != null ? obj : default(T);
                        Session.Set<T>(key,result);
                    }
                }
            }
            return result;
        }




    }
}








   