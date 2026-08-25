using Domain.Entities;
using Domain.Enums;
using DTO.User;
using FajrLog.Enum;
using Microsoft.AspNetCore.Http;
using Services.SessionServices;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Services.RedisService
{
    /// <summary>
    /// مدیریت لاگ ها درون ردیس
    /// </summary>
    public static class RedisLogManager
    {
        /// <summary>
        /// کلید پیشفرض 
        /// </summary>
        public static readonly string Key = "Logs";

        /// <summary>
        /// مدت زمان ماندگاری در ردیس
        /// </summary>
        //public static readonly int ExpMin = 1440;



        #region گرفتن لاگ از ردیس
        /// <summary>
        /// گرفتن همه لاگ ها از ردیس
        /// <para>
        /// ایتم های لاگ همچنان در ردیس می ماند.
        /// </para>
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        public static async Task<List<UserLog>> GetAllLogs(this IRedisDatabase db)
        {
            try
            {
                var logs = await db.SetMembersAsync<UserLog>(Key);
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
        public static async Task<List<UserLog>> PopLogs(this IRedisDatabase db, int Count = 100)
        {
            try
            {
                var logs = await db.SetPopAsync<UserLog>(Key, Count);
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
        public static async Task<bool> SetLog(this IRedisDatabase db, UserLog log)
        {
            try
            {
                return await db.SetAddAsync<UserLog>(Key, log);
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
        public static async Task<bool> SetLogs(this IRedisDatabase db, List<UserLog> logs)
        {
            try
            {
                var x = await db.SetAddAllAsync<UserLog>(Key, CommandFlags.None, logs.ToArray());
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
        public static async Task<bool> SetLog(this IRedisDatabase db, IHttpContextAccessor _contextAccessor, ActionType actionType, MenuType menuType,
            bool isSuccess = true, string description = null, long? targetId = null, FajrActionType? fajrActionType = null)
        {
            var user = _contextAccessor.HttpContext?.Session?.GetUser();
            var log = new UserLog(_contextAccessor, actionType, menuType, user?.FullName, user?.Id, isSuccess, description, targetId);
            var res = await db.SetLog(log);

            #region لاگ فجر
            if (fajrActionType != null)
                await db.SetFajrLog(_contextAccessor, (FajrActionType)fajrActionType, isSuccess ? FajrActionFlag.Success : FajrActionFlag.UnSuccess, FajrActionSensitivity.Info, description, targetId);
            #endregion
            
            return res;
        }



        /// <summary>
        /// افزودن لاگ api ردیس بدون کامیت
        /// </summary>
        /// <param name="_contextAccessor"></param>
        /// <param name="actionType"></param>
        /// <param name="menuType"></param>
        /// <param name="fullName"></param>
        /// <param name="userId"></param>
        /// <param name="isSuccess"></param>
        /// <param name="description"></param>
        public static async Task<bool> SetLog(this IRedisDatabase db, IHttpContextAccessor _contextAccessor, ActionType actionType, MenuType menuType,
            string fullName = null, long? userId = null, bool isSuccess = true, string description = null, long? targetId = null, bool IsApi = false, FajrActionType? fajrActionType = null)
        {
            var log = new UserLog(_contextAccessor, actionType, menuType, fullName, userId, isSuccess, description, targetId, IsApi);

            var res = await db.SetLog(log);
            return res;
        }


        /// <summary>
        /// افزودن لاگ لاگین به ردیس برای لاگین
        /// </summary>
        /// <param name="_contextAccessor"></param>
        /// <param name="fullName">نام کاربری لاگین کننده</param>
        /// <param name="isSuccess">وضعیت لاگین</param>
        /// <param name="description">توضیحات</param>
        public static async Task<bool> SetLoginLog(this IRedisDatabase db, IHttpContextAccessor _contextAccessor, FajrActionType fajrActionType, string username, string fullName = null, long? userId = null,
            bool isSuccess = true, string description = null, bool IsLogOut = false, bool IsApi = false)
        {
            bool res = true;
            if (fajrActionType != FajrActionType.logInSecurePage) {
                var log = new UserLog(_contextAccessor, username, userId, isSuccess, description, IsLogOut, IsApi);
                res = await db.SetLog(log);
            }
            await db.SetLoginFajrLog(_contextAccessor, fajrActionType, isSuccess ? FajrActionFlag.Success : FajrActionFlag.UnSuccess, FajrActionSensitivity.Info,username, userId, fullName, description);
            return res;
        }
        #endregion




        #region حذف لاگ از ردیس
        /// <summary>
        /// حذف یک لاگ از ردیس
        /// </summary>
        /// <param name="db">دیتابیس ردیس</param>
        /// <param name="storeId">آیدی فروشگاه</param>
        /// <returns></returns>
        public static async Task<bool> RemoveLog(this IRedisDatabase db, UserLog log)
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
        public static async Task<bool> RemoveLogs(this IRedisDatabase db, List<UserLog> logs)
        {
            try
            {
                //var x = await db.SetRemoveAllAsync(Key, CommandFlags.None, logs);
                foreach (var item in logs)
                    await db.RemoveLog(item);
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
