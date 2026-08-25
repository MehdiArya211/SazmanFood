using BLL.Interface;
using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using DTO.DataTable;
using DTO.UserLog;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Services.SessionServices;

namespace BLL
{
    public class UserLogManager : Manager<UserLog, LogContext>, IUserLogManager
    {
        public UserLogManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {
        }




        /// <summary>
        /// گرفتن لیست لاگ ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<UserActionLogDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserActionLogFilter filters)
        {
            return UOW.UserLogs.GetDataTableDTO(searchData, filters);
        }




        /// <summary>
        /// خروجی اکسل لاگ ها
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public List<UserActionLogDataTableDTO> GetReportExcel(UserActionLogFilter filters)
        {
            return UOW.UserLogs.GetReportExcel(filters);
        }





        /// <summary>
        /// گرفتن لیست لاگ لاگین ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<UserLoginLogDataTableDTO> GetLoginDataTableDTO(DataTableSearchDTO searchData, UserLoginLogFilter filters)
        {
            return UOW.UserLogs.GetLoginDataTableDTO(searchData, filters);
        }











        /// <summary>
        /// خروجی اکسل لاگین ها
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public List<UserLoginLogDataTableDTO> GetReportExcel(UserLoginLogFilter filters)
        {
            return UOW.UserLogs.GetReportExcel(filters);
        }




        /// <summary>
        /// کاربر با توجه به دفعات تلاش برای لاگین، مجاز به لاگین است؟
        /// <para>5 تلاش ناموفق منجر به مسدود شدن 20 دقیقه ای خواهد شد.</para>
        /// </summary>
        /// <param name="Username">نام کاربری</param>
        /// <returns></returns>
        public BaseResult UserCanLogin(string Username)
        {
            var date = DateTime.Now.AddMinutes(-20);
            var last5Login = UOW.UserLogs.Get(x => x.ActionType == ActionType.Login && x.MenuType == MenuType.Login && x.CreateDate > date && x.FullName == Username, take: 5).ToList();
            if (last5Login == null || last5Login.Count() < 5 || last5Login.Any(x => x.IsSuccess))
                return new BaseResult(true, "کاربر مجاز به لاگین است.");

            var diff = DateTime.Now - last5Login.Last().CreateDate;
            return new BaseResult(false, $"حساب کاربری شما بدلیل ورود اشتباه کلمه عبور تا {diff.TotalMinutes} دقیقه آینده مسدود می باشد.");

        }




        /// <summary>
        /// گرفتن آخرین لاگ لاگین کاربر
        /// </summary>
        /// <param name="UserId">آیدی کاربر</param>
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// <returns></returns>
        public UserLog GetUserLastLogin(string Username)
        {
            //return null;
            return UOW.UserLogs.Get(x => x.IsSuccess && x.FullName == Username
            && x.ActionType == ActionType.Login && x.MenuType == MenuType.Login,
            x => x.OrderByDescending(z => z.Id), 1, 1)
                .FirstOrDefault();
        }





    }
}