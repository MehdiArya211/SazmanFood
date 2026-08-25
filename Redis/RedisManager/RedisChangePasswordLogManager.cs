using DTO.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Services.RedisService
{
    /// <summary>
    /// بررسی تعداد تلاشهای کاربر برای تغیرر پسورد و بلاک کردن بعد از 5 بار تلاش نا موفق
    /// </summary>
    public static class RedisChangePasswordLogManager
    {
        /// <summary>
        /// کلید پیشفرض 
        /// </summary>
        public static readonly string Key = "ChangePassword:";

        /// <summary>
        /// مدت زمان ماندگاری در ردیس
        /// </summary>
        public static readonly int ExpMin = 20;



        /// <summary>
        /// گرفتن رکورد مربوط به یوزرنیم خاص
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="Username">نام کاربری</param>
        public static async Task<LoginLogDTO> GetChangePasswordLog(this IRedisDatabase db, string Username)
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
        /// <param name="Username">کد کپچا تولید شده</param>
        /// <param name="expMin">مدت زمان اعتبار کپچا</param>
        /// <returns></returns>
        public static async Task<LoginLogDTO> SetChangePasswordLog(this IRedisDatabase db, string Username, int? expMin = null)
        {
            try
            {
                var log = await db.GetChangePasswordLog(Username);
                if (log != null)
                {
                    await db.RemoveChangePasswordLog(Username);
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
        public static async Task<bool> RemoveChangePasswordLog(this IRedisDatabase db, string Username)
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
