using Domain.Entities;
using Domain.Enums;
using DTO.DataTable;
using DTO.UserLog;
using Microsoft.AspNetCore.Http;

namespace DAL.Interface
{
    public interface IUserLogRepository : IRepository<UserLog>
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
        /// افزودن لاگ به کانتکست بدون کامیت
        /// </summary>
        /// <param name="_contextAccessor"></param>
        /// <param name="actionType"></param>
        /// <param name="menuType"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <param name="isSuccess"></param>
        /// <param name="description"></param>
        void Add(IHttpContextAccessor _contextAccessor, ActionType actionType, MenuType menuType,
            string userName = null, long? userId = null, bool isSuccess = true, string description = null);

        /// <summary>
        /// افزودن لاگ به کانتکست بدون کامیت
        /// </summary>
        /// <param name="_contextAccessor"></param>
        /// <param name="userName"></param>
        /// <param name="isSuccess"></param>
        /// <param name="description"></param>
        void Add(IHttpContextAccessor _contextAccessor, string userName, long? userId = null, bool isSuccess = true, string description = null);
    }
}