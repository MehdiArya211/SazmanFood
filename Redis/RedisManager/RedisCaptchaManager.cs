using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Api.Services.RedisService
{
    /// <summary>
    /// api - مدیریت کپچا درون ردیس
    /// </summary>
    public static class RedisCaptchaManager
    {
        /// <summary>
        /// کلید پیشفرض
        /// </summary>
        public static readonly string Key = "Api_Captcha:";

        /// <summary>
        /// مدت زمان ماندگاری در ردیس
        /// </summary>
        public static readonly int ExpMin = 30;

        /// <summary>
        /// گرفتن کپچا از ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="captcha">کد کپچا ایجاد شده </param>
        /// <returns>آیپی درخواست دهسنده کپچا</returns>
        public static async Task<string> GetCaptcha(this IRedisDatabase db, string captcha)
        {
            try
            {
                return await db.GetAsync<string>(Key + captcha);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// بررسی وجود تکراری بودن کپچا
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="captcha">کد کپچا ایجاد شده </param>
        /// <returns></returns>
        public static async Task<bool> CaptchaIsExist(this IRedisDatabase db, string captcha)
        {
            try
            {
                var ip = await db.GetCaptcha(captcha);
                return !string.IsNullOrEmpty(ip);
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// افزودن کپچا به ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="captcha">کد کپچا تولید شده</param>
        /// <param name="ip">آیپی کاربر درخواست دهنده کپچا</param>
        /// <param name="expMin">مدت زمان اعتبار کپچا</param>
        /// <returns></returns>
        public static async Task<bool> SetCaptcha(this IRedisDatabase db, string captcha, string ip, int? expMin = null)
        {
            try
            {
                return await db.AddAsync(Key + captcha, ip, DateTimeOffset.Now.AddMinutes(expMin ?? ExpMin));
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// حذف کپچا از ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="captcha">کد کپچا تولید شده</param>
        /// <returns></returns>
        public static async Task<bool> RemoveCaptcha(this IRedisDatabase db, string captcha)
        {
            try
            {
                return await db.RemoveAsync(Key + captcha);
            }
            catch
            {
                return false;
            }
        }


    }
}
