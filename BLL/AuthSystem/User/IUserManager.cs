using Domain.Entities;
using Domain.Enums;
using DTO;
using DTO.Base;
using DTO.DataTable;
using DTO.User;
using Infrastructure.Data;

namespace BLL.Interface
{
    public interface IUserManager : IManager<User, ApplicationContext>
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
        UserBaseInfoDTO GetUserBaseInfo(long? id);




        /// <summary>
        /// ایجاد کاربر جدید
        /// </summary>
        /// <param name="model">مدل اطلاعات ادمین</param>
        /// <returns></returns>
        BaseResult Create(UserCreateDTO model);



        /// <summary>
        /// گرفتن مدل لازم برای ویرایش کاربر
        /// </summary>
        /// <param name="id">آیدی کاربر</param>
        /// <returns></returns>
        UserEditDTO GetEditDTO(long? id);



        /// <summary>
        /// ویرایش کاربر
        /// </summary>
        /// <param name="model">اطلاعات کاربر</param>
        /// <returns></returns>
        BaseResult Update(UserEditDTO model);



        /// <summary>
        /// گرفتن مدل لازم برای ویرایش پروفایل توسط کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <returns></returns>
        UserEditProfileDTO GetEditProfileDTO(long? id);



        /// <summary>
        /// ویرایش پروفایل
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        BaseResult UpdateProfile(UserEditProfileDTO model);



        /// <summary>
        /// تلفن معتبر است؟
        /// </summary>
        /// <param name="Mobile">تلفن</param>
        /// <returns></returns>
        bool MobileIsUnique(string Mobile, long? Id);



        /// <summary>
        /// نام کاربری معتبر است؟
        /// </summary>
        /// <param name="Username">نام کاربری</param>
        /// <returns></returns>
        bool UsernameIsUnique(string Username, long? Id);



        /// <summary>
        ///  گرفتن تعداد کل کاربران
        /// </summary>
        /// <returns></returns>
        int GetTotalCount();




        
        /// <summary>
        /// تغییر کلمه عبور توسط خود کاربر
        /// </summary>
        /// <param name="model">مدل تغییر کلمه عبور</param>
        /// <returns></returns>
        BaseResult ProfileChangePassword(UserProfileChangePasswordDTO model);


                          
        /// <summary>
        /// تغییر کلمه عبور کاربر توسط ادمین
        /// </summary>
        /// <param name="model"> مدل تغییر کلمه عبور</param>
        /// <returns></returns>
        BaseResult ChangePassword(UserChangePasswordDTO model);



        /// <summary>
        /// جستجوی کاربران
        /// </summary>
        /// <param name="word">بخشی از نام یا تلفن</param>
        /// <returns></returns>
        List<UserSearchDTO> SearchUsers(string word);



        /// <summary>
        /// جستجوی کاربران یک فروشگاه 
        /// </summary>
        /// <param name="StoreId">شناسه فروشگاه</param>
        /// <returns></returns>
        List<UserSearchDTO> GetUsersForFactors(UserType? Type);



        /// <summary>
        /// جستجو کاربران برای سلکتایز
        /// </summary>
        /// <param name="word">متن جستجو</param>
        List<SelectListDTO> Search(string word);

        long GetUserIdWithUserName(string UserName);

        /// <summary>
        /// بررسی وجود کاربر محلی با نام کاربری.
        /// </summary>
        bool ExistsByUsername(string username);

        /// <summary>
        /// ایجاد خودکار رزروکننده غذا از اطلاعات سرویس پرسنلی.
        /// </summary>
        BaseResult CreateMealBookerFromPersonnel(PersonalInfDTO person, string initialPassword);
        //Task<string> SetLoginToken(long userId, int? expMin = null);



    }
}
