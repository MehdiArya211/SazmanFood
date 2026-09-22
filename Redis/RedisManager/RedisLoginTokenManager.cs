using Domain.Entities;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Services.RedisService
{
    /// <summary>
    /// ذخیره توکن کاربر برای جلوگیری لاگین همزمان 2 نفر با یک اکانت
    /// </summary>
    public static class RedisLoginTokenManager
    {
        /// <summary>
        /// کلید پیشفرض 
        /// </summary>
        public static readonly string Key = "UserToken:";
        public static readonly string UserDeviceDataKey = "UserDeviceData:";

        /// <summary>
        /// مدت زمان ماندگاری در ردیس
        /// </summary>
        public static readonly int ExpMin = 300;

        /// <summary>
        /// گرفتن رکورد مربوط به یوزرنیم خاص
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="UserId">نام کاربری</param>
        public static async Task<string> GetLoginToken(this IRedisDatabase db, long UserId)
        {
            try
            {
                return await db.GetAsync<string>(Key + UserId);
            }
            catch
            {
                return null;
            }
        }




        /// <summary>
        /// افزودن اطلاعات نام کاربری به ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="UserId">نام کاربری</param>
        /// <param name="expMin">مدت زمان اعتبار</param>
        /// <returns></returns>
        public static async Task<string> SetLoginToken(this IRedisDatabase db, long UserId, int? expMin = null)
        {
            try
            {
                var token = await db.GetLoginToken(UserId);
                if (!string.IsNullOrEmpty(token))
                    await db.RemoveLoginToken(UserId);

                token = Guid.NewGuid().ToString();
                var isSuccess = await db.AddAsync(Key + UserId, token, DateTimeOffset.Now.AddMinutes(expMin ?? ExpMin));
                if(isSuccess)
                    return token;
                return null;
            }
            catch
            {
                return null;
            }
        }




        /// <summary>
        /// حذف لاگ نام کاربری از ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="UserId">نام کاربری</param>
        /// <returns></returns>
        public static async Task<bool> RemoveLoginToken(this IRedisDatabase db, long UserId)
        {
            try
            {
                return await db.RemoveAsync(Key + UserId);
            }
            catch
            {
                return false;
            }
        }

        #region ورود به تغذیه و چاپ ژتون
        public static async Task<bool> RefreshUserSession(this IRedisDatabase db, long userId, long deviceId, int expMin = 3)
        {
            try
            {
                var key = $"UserSession:{deviceId}:{userId}";
                return await db.UpdateExpiryAsync(key, DateTimeOffset.Now.AddMinutes(expMin));
            }
            catch
            {
                return false;
            }
        }

        #endregion




    }
}
