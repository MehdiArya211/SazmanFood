using DTO.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Services.RedisService
{
    /// <summary>
    /// مدیریت منو هایی که نیاز به لاگین مجدد دارند
    /// </summary>
    public static class RedisReAuthorizeMenuManager
    {
        /// <summary>
        /// کلید پیشفرض 
        /// </summary>
        public static readonly string Key = "ReAuthorize:Menu{0}:User{1}";

        /// <summary>
        /// مدت زمان ماندگاری در ردیس
        /// </summary>
        public static readonly int ExpMin = 20;


        /// <summary>
        /// گرفتن کلید با ایدی منو و کاربر
        /// </summary>
        /// <param name="MenuId">ایدی منو</param>
        /// <param name="UserId">ایدی یوزر</param>
        /// <returns></returns>
        private static string GetKey(long MenuId, long UserId)
        {
            return string.Format(Key, MenuId, UserId);
        }



        /// <summary>
        /// آیا کاربر برای منو مشخص شده احراز هویت مجدد کرده است؟
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="MenuId">آیدی منو</param>
        /// <param name="UserId">آیدی کاربر</param>
        public static async Task<bool> UserHasReAuthorizeMenu(this IRedisDatabase db, long MenuId, long UserId)
        {
            try
            {
                var _key = GetKey(MenuId, UserId);
                return await db.GetAsync<bool>(_key);
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// تنظیم احراز هویت مجدد کاربر برای منو خاص
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="MenuId">آیدی منو</param>
        /// <param name="UserId">آیدی کاربر</param>
        /// <returns></returns>
        public static async Task SetUserReAuthorizeMenu(this IRedisDatabase db, long MenuId, long UserId, int? expMin = null)
        {
            try
            {
                var _key = GetKey(MenuId, UserId);                
                await db.AddAsync(_key, true, DateTimeOffset.Now.AddMinutes(expMin ?? ExpMin));
            }
            catch
            {
                return;
            }
        }




        /// <summary>
        /// حذف احراز هویت مجدد کاربر برای منو خاص
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="MenuId">آیدی منو</param>
        /// <param name="UserId">آیدی کاربر</param>
        /// <returns></returns>
        public static async Task<bool> RemoveUserReAuthorizeMenu(this IRedisDatabase db, long MenuId, long UserId)
        {
            try
            {
                var _key = GetKey(MenuId, UserId);
                return await db.RemoveAsync(_key);
            }
            catch
            {
                return false;
            }
        }




    }
}
