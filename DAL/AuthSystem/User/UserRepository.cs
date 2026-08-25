using DAL.Interface;
using Domain.Entities;
using DTO.DataTable;
using DTO.User;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Z.EntityFramework.Plus;

namespace DAL
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DbContext _Context) : base(_Context)
        {
        }




        /// <summary>
        /// گرفتن لیست کاربران برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<UserDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserFilterDataTableDTO filters)
        {
            var model = new DataTableResponseDTO<UserDataTableDTO>();

            var recordTotal = Entities.DeferredCount().FutureValue();

            #region شرط ها

            var filter = PredicateBuilder.New<User>(true);


         
            // نام کاربری
            if (!string.IsNullOrEmpty(filters.Username))
                filter.And(x => x.Username.Contains(filters.Username));

            //فعال
            if (filters.IsEnabled != null)
                filter.And(x => x.IsEnabled == filters.IsEnabled);

            //نقش
            if (filters.RoleId != null)
                filter.And(x => x.RoleId == filters.RoleId);

            // نام
            if (!string.IsNullOrEmpty(filters.Name))
                filter.And(x => x.Name.Contains(filters.Name));

            // موبایل
            if (!string.IsNullOrEmpty(filters.Mobile))
                filter.And(x => x.Mobile.Contains(filters.Mobile));

            // تاریخ شروع
            if (filters.StartDate != null)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0);
                filters.StartDate = ((DateTime)filters.StartDate).Date + ts;
                filter.And(x => x.CreateDate >= filters.StartDate);
            }

            // تاریخ پایان
            if (filters.EndDate != null)
            {
                TimeSpan ts = new TimeSpan(23, 23, 23);
                filters.EndDate = ((DateTime)filters.EndDate).Date + ts;
                filter.And(x => x.CreateDate <= filters.EndDate);
            }

            //search
            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue;
                filter.And(s => s.Username.Contains(srch));
            }


            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            //sorting and paging
            var sortCol = searchData.sortColumnName;
            var selectedModel = Entities.Where(filter)
                                        .OrderBy(sortCol + " " + searchData.sortDirection)
                                        .Skip(searchData.start)
                                        .Take(searchData.length)
                                        .Select(UserDataTableDTO.Selector)
                                        .Future();

       model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
        }







        /// <summary>
        /// گرفتن اطلاعات پایه کاربر با آیدی
        /// </summary>
        /// <param name="id">آیدی کاربر</param>
        /// <returns></returns>
        public UserBaseInfoDTO GetUserBaseInfo(long id)
        {
            return GetOneDTO<UserBaseInfoDTO>(UserBaseInfoDTO.Selector, u => u.Id == id);
        }




        /// <summary>
        /// تلفن همراه معتبر است؟
        /// </summary>
        /// <param name="Mobile">تلفن همراه</param>
        /// <param name="Id">آیدی کاربر</param>
        /// <returns></returns>
        public bool MobileIsUnique(string Mobile, long? Id = null)
        {
            if (Id == null)
                return !Any(x => x.Mobile == Mobile);
            return !Any(x => x.Mobile == Mobile && x.Id != Id);
        }




        /// <summary>
        /// نام کاربری معتبر است؟
        /// </summary>
        /// <param name="Username">نام کاربری</param>
        /// <param name="Id">آیدی کاربر</param>
        /// <returns></returns>
        public bool UsernameIsUnique(string Username, long? Id = null)
        {
            if (Id == null)
                return !Any(x => x.Username == Username);
            return !Any(x => x.Username == Username && x.Id != Id);
        }

    }
}
