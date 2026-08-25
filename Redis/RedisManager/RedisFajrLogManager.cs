using Domain.Entities;
using Domain.Enums;
using FajrLog.Domain;
using FajrLog.DTO;
using FajrLog.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.SessionServices;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Services.RedisService
{
    /// <summary>
    /// مدیریت لاگ های فجر درون ردیس
    /// </summary>
    public static class RedisFajrLogManager
    {
        /// <summary>
        /// کلید پیشفرض 
        /// </summary>
        public static readonly string Key = "FajrLogs";
        public static readonly string LogCountKey = "FajrLogCount";


        #region گرفتن کانفیگ در کلاس استاتیک
        private static IConfiguration config;
        public static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                config = builder.Build();
                return config;
            }
        }
        #endregion




        #region تعداد لاگ فجر

        /// <summary>
        /// بروزرسانی تعداد لاگ های فجر که همانند آیدی لاگ است و هربار یکی اضافه میشود
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static async Task<long> GetFajrLogId(this IRedisDatabase db)
        {
            try
            {
                var count = await db.GetAsync<long?>(LogCountKey) ?? 0;
                count++;
                await db.AddAsync<long?>(LogCountKey, count);
                return count;
            }
            catch
            {
                return 0;
            }
        }
        #endregion



        #region گرفتن لاگ از ردیس
        /// <summary>
        /// گرفتن همه لاگ ها از ردیس
        /// <para>
        /// ایتم های لاگ همچنان در ردیس می ماند.
        /// </para>
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        public static async Task<List<FajrLogEntity>> GetAllFajrLogs(this IRedisDatabase db)
        {
            try
            {
                var logs = await db.SetMembersAsync<FajrLogEntity>(Key);
                return logs.ToList();
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// گرفتن لاگ ها از ردیس به تعداد دلخواه
        /// <para>
        /// ایتم های لاگ پس از دریافت از ردیس حذف خواهند شد.
        /// </para>
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="Count">تعداد آیتم درخواستی</param>
        public static async Task<List<FajrLogEntity>> PopFajrLogs(this IRedisDatabase db, int Count = 100)
        {
            try
            {
                var logs = await db.SetPopAsync<FajrLogEntity>(Key, Count);
                return logs.ToList();
            }
            catch
            {
                return null;
            }
        }
        #endregion



        #region افزودن لاگ به ردیس

        /// <summary>
        /// افزودن لاگ به ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="log">ایتم لاگ</param>
        /// <returns></returns>
        public static async Task<bool> SetFajrLog(this IRedisDatabase db, FajrLogEntity log)
        {
            try
            {
                var id = await db.GetFajrLogId();
                log.logId = id.ToString();
                log.logNum = id;
                return await db.SetAddAsync<FajrLogEntity>(Key, log);
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// افزودن لاگ ها به ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="log">ایتم لاگ</param>
        /// <returns></returns>
        public static async Task<bool> SetFajrLogs(this IRedisDatabase db, List<FajrLogEntity> logs)
        {
            try
            {
                var x = await db.SetAddAllAsync<FajrLogEntity>(Key, CommandFlags.None, logs.ToArray());
                return x > 0;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// افزودن لاگ پنل ادمین به ردیس بدون کامیت
        /// </summary>
        /// <param name="_contextAccessor"></param>
        /// <param name="actionType"></param>
        /// <param name="menuType"></param>
        /// <param name="isSuccess"></param>
        /// <param name="description"></param>
        public static async Task<bool> SetFajrLog(this IRedisDatabase db, IHttpContextAccessor _contextAccessor, FajrActionType actionType, FajrActionFlag flag, FajrActionSensitivity sensitivity,
            string description = null, long? targetId = null)
        {
            var user = _contextAccessor.HttpContext?.Session?.GetUser();
            var baseInfo = new FajrLogBaseDTO();
            Configuration.GetSection("Fajr").Bind(baseInfo);
           // var log = new FajrLogEntity(_contextAccessor, baseInfo, actionType, flag, sensitivity, user?.Username, user?.Id, user?.FullName, description, targetId);
           // var res = await db.SetFajrLog(log);
            return true;
        }



        /// <summary>
        /// افزودن لاگ لاگین به ردیس برای لاگین
        /// </summary>
        /// <param name="_contextAccessor"></param>
        /// <param name="fullName">نام کاربری لاگین کننده</param>
        /// <param name="isSuccess">وضعیت لاگین</param>
        /// <param name="description">توضیحات</param>
        public static async Task<bool> SetLoginFajrLog(this IRedisDatabase db, IHttpContextAccessor _contextAccessor, FajrActionType actionType, FajrActionFlag flag, FajrActionSensitivity sensitivity,
            string username, long? userId = null, string FullName = null, string description = null)
        {
            var baseInfo = new FajrLogBaseDTO();
            Configuration.GetSection("Fajr").Bind(baseInfo);
           // var log = new FajrLogEntity(_contextAccessor, baseInfo, actionType, flag, sensitivity, username, userId, FullName, description, userId);
          //  var res = await db.SetFajrLog(log);
            return true;
        }
        #endregion




        #region حذف لاگ از ردیس
        /// <summary>
        /// حذف یک لاگ از ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="storeId">آیدی فروشگاه</param>
        /// <returns></returns>
        public static async Task<bool> RemoveFajrLog(this IRedisDatabase db, FajrLogEntity log)
        {
            try
            {
                return await db.SetRemoveAsync(Key, log);
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// حذف همه لاگ ها از ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="storeId">آیدی فروشگاه</param>
        /// <returns></returns>
        public static async Task<bool> RemoveFajrLogs(this IRedisDatabase db, List<FajrLogEntity> logs)
        {
            try
            {
                //var x = await db.SetRemoveAllAsync(Key, CommandFlags.None, logs);
                foreach (var item in logs)
                    await db.RemoveFajrLog(item);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion




    }
}
