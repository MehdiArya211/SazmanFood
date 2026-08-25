using DTO.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Services.RedisService
{
    /// <summary>
    /// بررسی تعداد لاگین کاربر های مختلف و بلاک کردن بعد از 5 بار تلاش نا موفق
    /// </summary>
    public static class RedisLoginLogManager
    {
        /// <summary>
        /// کلید پیشفرض 
        /// </summary>
        public static readonly string Key = "UserLogin:";

        /// <summary>
        /// مدت زمان ماندگاری در ردیس
        /// </summary>
        public static readonly int ExpMin = 20;



        /// <summary>
        /// گرفتن رکورد مربوط به یوزرنیم خاص
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="Username">نام کاربری</param>
        public static async Task<LoginLogDTO> GetLoginLog(this IRedisDatabase db, string Username)
        {
            try
            {
                return await db.GetAsync<LoginLogDTO>(Key + Username);
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
        /// <param name="Username">نام کاربری</param>
        /// <param name="expMin">مدت زمان اعتبار</param>
        /// <returns></returns>
        public static async Task<LoginLogDTO> SetLoginLog(this IRedisDatabase db, string Username, int? expMin = null)
        {
            try
            {
                var log = await db.GetLoginLog(Username);
                if (log != null)
                {
                    await db.RemoveLoginLog(Username);
                    log.Count += 1;
                    if(log.Count <= 5)
                        log.CreateDate = DateTime.Now;
                }
                else
                    log = new LoginLogDTO(1);

                var isSuccess = await db.AddAsync(Key + Username, log, DateTimeOffset.Now.AddMinutes(expMin ?? ExpMin));
                if(isSuccess)
                    return log;
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
        /// <param name="Username">نام کاربری</param>
        /// <returns></returns>
        public static async Task<bool> RemoveLoginLog(this IRedisDatabase db, string Username)
        {
            try
            {
                return await db.RemoveAsync(Key + Username);
            }
            catch
            {
                return false;
            }
        }




    }
}
