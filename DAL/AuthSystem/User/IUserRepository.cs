using Domain.Entities;
using DTO.DataTable;
using DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// گرفتن لیست کاربران برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        DataTableResponseDTO<UserDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserFilterDataTableDTO filters);



        /// <summary>
        /// گرفتن اطلاعات پایه کاربر با آیدی
        /// </summary>
        /// <param name="id">آیدی کاربر</param>
        /// <returns></returns>
        UserBaseInfoDTO GetUserBaseInfo(long id);


        /// <summary>
        /// تلفن همراه معتبر است؟
        /// </summary>
        /// <param name="Mobile">تلفن همراه</param>
        /// <returns></returns>
        bool MobileIsUnique(string Mobile, long? Id);


        /// <summary>
        /// نام کاربری معتبر است؟
        /// </summary>
        /// <param name="Username">نام کاربری</param>
        /// <returns></returns>
        bool UsernameIsUnique(string Username, long? Id);

    }
}
