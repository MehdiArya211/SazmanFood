using DAL.Interface;
using Domain.Entities;
using Domain.Enums;
using DTO.DataTable;
using DTO.UserLog;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Z.EntityFramework.Plus;

namespace DAL
{
    public class UserLogRepository : Repository<UserLog>, IUserLogRepository
    {
        public UserLogRepository(DbContext _Context) : base(_Context)
        {
        }

        /// <summary>
        /// گرفتن لیست لاگ ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DataTableResponseDTO<UserActionLogDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserActionLogFilter filters)
        {
            var model = new DataTableResponseDTO<UserActionLogDataTableDTO>();

            var filter = PredicateBuilder.New<UserLog>(true);

            #region شرط ها

            // بجز برای ورود و خروج کاربران
            filter.Start(x => x.ActionType != ActionType.Login && x.ActionType != ActionType.LogOff);

            var recordTotal = Entities.DeferredCount(filter).FutureValue();

            //search
            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue;
                filter = filter.And(s => s.Description.Contains(srch) ||
                                         s.FullName.Contains(srch));
            }
            #endregion


            #region فیلتر ها
            //فعال
            if (filters.IsSuccess != null)
                filter.And(x => x.IsSuccess == filters.IsSuccess);

            // متن
            if (!string.IsNullOrEmpty(filters.Description))
                filter.And(x => x.Description.Contains(filters.Description));

            // نوع عملیات
            if (filters.ActionType != null)
                filter.And(x => x.ActionType == filters.ActionType);

            // حوزه
            if (filters.MenuType != null)
                filter.And(x => x.MenuType == filters.MenuType);

            // موبایل
            if (!string.IsNullOrEmpty(filters.FullName))
                filter.And(x => x.FullName.Contains(filters.FullName));

            // تاریخ شروع
            if (filters.CreateStartDate != null)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0);
                filters.CreateStartDate = ((DateTime)filters.CreateStartDate).Date + ts;
                filter = filter.And(x => x.CreateDate >= filters.CreateStartDate);
            }

            // تاریخ پایان
            if (filters.CreateEndDate != null)
            {
                TimeSpan ts = new TimeSpan(23, 23, 23);
                filters.CreateEndDate = ((DateTime)filters.CreateEndDate).Date + ts;
                filter = filter.And(x => x.CreateDate <= filters.CreateEndDate);
            }
            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            //sorting and paging
            var sortCol = searchData.sortColumnName;
            var selectedModel = Entities.Where(filter)
                                        .OrderBy(sortCol + " " + searchData.sortDirection)
                                        .Skip(searchData.start)
                                        .Take(searchData.length)
                                        .Select(UserActionLogDataTableDTO.Selector)
                                        .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
        }




        /// <summary>
        /// خروجی اکسل لاگین ها
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public List<UserActionLogDataTableDTO> GetReportExcel(UserActionLogFilter filters)
        {
            var filter = PredicateBuilder.New<UserLog>(true);

            // بجز برای ورود و خروج کاربران
            filter.Start(x => x.ActionType != ActionType.Login && x.ActionType != ActionType.LogOff);

            #region فیلتر ها
            //فعال
            if (filters.IsSuccess != null)
                filter.And(x => x.IsSuccess == filters.IsSuccess);

            // متن
            if (!string.IsNullOrEmpty(filters.Description))
                filter.And(x => x.Description.Contains(filters.Description));

            // نوع عملیات
            if (filters.ActionType != null)
                filter.And(x => x.ActionType == filters.ActionType);

            // حوزه
            if (filters.MenuType != null)
                filter.And(x => x.MenuType == filters.MenuType);

            // موبایل
            if (!string.IsNullOrEmpty(filters.FullName))
                filter.And(x => x.FullName.Contains(filters.FullName));

            // تاریخ شروع
            if (filters.CreateStartDate != null)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0);
                filters.CreateStartDate = ((DateTime)filters.CreateStartDate).Date + ts;
                filter = filter.And(x => x.CreateDate >= filters.CreateStartDate);
            }

            // تاریخ پایان
            if (filters.CreateEndDate != null)
            {
                TimeSpan ts = new TimeSpan(23, 23, 23);
                filters.CreateEndDate = ((DateTime)filters.CreateEndDate).Date + ts;
                filter = filter.And(x => x.CreateDate <= filters.CreateEndDate);
            }
            #endregion

            var model = Entities.Where(filter)
                                        .Select(UserActionLogDataTableDTO.Selector)
                                        .Future();

            return model.ToList();
        }




        /// <summary>
        /// گرفتن لیست لاگ لاگین ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<UserLoginLogDataTableDTO> GetLoginDataTableDTO(DataTableSearchDTO searchData, UserLoginLogFilter filters)
        {
            var model = new DataTableResponseDTO<UserLoginLogDataTableDTO>();

            var filter = PredicateBuilder.New<UserLog>(true);

            #region شرط ها

            // فقط برای ورود و خروج کاربران
            filter.Start(x => x.ActionType == ActionType.Login || x.ActionType == ActionType.LogOff);

            // برای کاربر جاری
            if (filters.UserId != null)
                filter.And(x => x.UserId == filters.UserId);

            var recordTotal = Entities.DeferredCount(filter).FutureValue();


            //search
            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue;
                filter = filter.And(s => s.Description.Contains(srch) || s.FullName.Contains(srch));
            }
            #endregion


            #region فیلتر ها
            //فعال
            if (filters.IsSuccess != null)
                filter.And(x => x.IsSuccess == filters.IsSuccess);

            // متن
            if (!string.IsNullOrEmpty(filters.Description))
                filter.And(x => x.Description.Contains(filters.Description));

            // موبایل
            if (!string.IsNullOrEmpty(filters.FullName))
                filter.And(x => x.FullName.Contains(filters.FullName));

            // تاریخ شروع
            if (filters.CreateStartDate != null)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0);
                filters.CreateStartDate = ((DateTime)filters.CreateStartDate).Date + ts;
                filter = filter.And(x => x.CreateDate >= filters.CreateStartDate);
            }

            // تاریخ پایان
            if (filters.CreateEndDate != null)
            {
                TimeSpan ts = new TimeSpan(23, 23, 23);
                filters.CreateEndDate = ((DateTime)filters.CreateEndDate).Date + ts;
                filter = filter.And(x => x.CreateDate <= filters.CreateEndDate);
            }
            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            //sorting and paging
            var sortCol = searchData.sortColumnName;
            var selectedModel = Entities.Where(filter)
                                        .OrderBy(sortCol + " " + searchData.sortDirection)
                                        .Skip(searchData.start)
                                        .Take(searchData.length)
                                        .Select(UserLoginLogDataTableDTO.Selector)
                                        .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
        }





        /// <summary>
        /// خروجی اکسل لاگین ها
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public List<UserLoginLogDataTableDTO> GetReportExcel(UserLoginLogFilter filters)
        {
            var filter = PredicateBuilder.New<UserLog>(true);

            // فقط برای ورود و خروج کاربران
            filter.Start(x => x.ActionType == ActionType.Login || x.ActionType == ActionType.LogOff);

            #region فیلتر ها
            //فعال
            if (filters.IsSuccess != null)
                filter.And(x => x.IsSuccess == filters.IsSuccess);

            // متن
            if (!string.IsNullOrEmpty(filters.Description))
                filter.And(x => x.Description.Contains(filters.Description));

            // موبایل
            if (!string.IsNullOrEmpty(filters.FullName))
                filter.And(x => x.FullName.Contains(filters.FullName));

            // تاریخ شروع
            if (filters.CreateStartDate != null)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0);
                filters.CreateStartDate = ((DateTime)filters.CreateStartDate).Date + ts;
                filter = filter.And(x => x.CreateDate >= filters.CreateStartDate);
            }

            // تاریخ پایان
            if (filters.CreateEndDate != null)
            {
                TimeSpan ts = new TimeSpan(23, 23, 23);
                filters.CreateEndDate = ((DateTime)filters.CreateEndDate).Date + ts;
                filter = filter.And(x => x.CreateDate <= filters.CreateEndDate);
            }
            #endregion

            var model = Entities.Where(filter)
                                        .Select(UserLoginLogDataTableDTO.Selector)
                                        .Future();

            return model.ToList();
        }




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
        public void Add(IHttpContextAccessor _contextAccessor, ActionType actionType, MenuType menuType,
            string userName = null, long? userId = null, bool isSuccess = true, string description = null)
        {
            var log = new UserLog(_contextAccessor, actionType, menuType, userName, userId, isSuccess, description);
            Context.Add(log);
        }



        /// <summary>
        /// افزودن لاگ به کانتکست بدون کامیت
        /// </summary>
        /// <param name="_contextAccessor"></param>
        /// <param name="userName"></param>
        /// <param name="isSuccess"></param>
        /// <param name="description"></param>
        public void Add(IHttpContextAccessor _contextAccessor, string userName, long? userId = null, bool isSuccess = true, string description = null)
        {
            var log = new UserLog(_contextAccessor, userName, userId, isSuccess, description);
            Context.Add(log);
        }

    }
}