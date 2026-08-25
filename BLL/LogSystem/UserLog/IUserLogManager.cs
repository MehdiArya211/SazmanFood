using Domain.Entities;
using DTO.Base;
using DTO.DataTable;
using DTO.UserLog;
using Infrastructure.Data;

namespace BLL.Interface
{
    public interface IUserLogManager : IManager<UserLog, LogContext>
    {
        /// <summary>
        /// گرفتن لیست لاگ ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        DataTableResponseDTO<UserActionLogDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserActionLogFilter filters);





        /// <summary>
        /// خروجی اکسل لاگ ها
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<UserActionLogDataTableDTO> GetReportExcel(UserActionLogFilter filters);




        /// <summary>
        /// گرفتن لیست لاگ لاگین ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        DataTableResponseDTO<UserLoginLogDataTableDTO> GetLoginDataTableDTO(DataTableSearchDTO searchData, UserLoginLogFilter filters);





        /// <summary>
        /// خروجی اکسل لاگین ها
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<UserLoginLogDataTableDTO> GetReportExcel(UserLoginLogFilter filters);




        /// <summary>
        /// کاربر با توجه به دفعات تلاش برای لاگین، مجاز به لاگین است؟
        /// <para>5 تلاش ناموفق منجر به مسدود شدن 20 دقیقه ای خواهد شد.</para>
        /// </summary>
        /// <param name="Username">نام کاربری</param>
        /// <returns></returns>
        BaseResult UserCanLogin(string Username);



        /// <summary>
        /// گرفتن آخرین لاگ لاگین کاربر
        /// </summary>
        /// <param name="UserId">آیدی کاربر</param>
        /// <returns></returns>
        UserLog GetUserLastLogin(string Username);


    }
}