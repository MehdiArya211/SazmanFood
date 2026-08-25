using Domain.Entities;
using DTO.Entities.MaxaRabbitMQ;
using DTO.User;
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

        public static async Task<AccessLogMonitoringEvent?> GetRedisUserDeviceData(
            this IRedisDatabase db, long DeviceId)
        {
            try
            {
                return await db.GetAsync<AccessLogMonitoringEvent>(UserDeviceDataKey + 1);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<AccessLogMonitoringEvent> SetRedisUserDeviceData(this IRedisDatabase db,
            AccessLogMonitoringEvent monitoringEvent, int? expMin = null)
        {
            try
            {
               // monitoringEvent.UserId = 95003599; // TODO: DELETE it later
                monitoringEvent.UserId =long.Parse(monitoringEvent.UniqueID) ; 

                var user = await db.GetRedisUserDeviceData(1);
                if (user != null)
                {
                    await db.RemoveAsync(UserDeviceDataKey + 1);
                }

                //var isSuccess = await db.AddAsync(UserDeviceDataKey + monitoringEvent.DeviceId,
                //    monitoringEvent,
                //    DateTimeOffset.Now.AddMinutes(expMin ?? ExpMin));

                var isSuccess = await db.AddAsync(UserDeviceDataKey + 1,
    monitoringEvent,
    DateTimeOffset.Now.AddMinutes(expMin ?? 5)); // 🟢 بجای 300 دقیقه، بذار 3 دقیقه

                if (isSuccess)
                    return monitoringEvent;
                return null;
            }
            catch
            {
                return null;
            }
        }




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
