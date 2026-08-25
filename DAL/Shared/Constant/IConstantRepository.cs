using Domain.Entities;
using Domain.Enums;
using DTO.DataTable;

namespace DAL.Interface
{
    public interface IConstantRepository : IRepository<Constant>
    {
        /// <summary>
        /// گرفتن لیست برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        DataTableResponseDTO<Constant> GetDataTableDTO(DataTableSearchDTO searchData);





        /// <summary>
        /// گرفتن تعداد دفعات مجاز برای لاگین ناموفق
        /// </summary>
        /// <returns></returns>
        int GetFailedLoginCount();





        /// <summary>
        /// کاربر هر چند روز کلمه عبور را تغییر دهد
        /// <para>
        /// مقدار پیشفرض 60 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        int? GetChangePasswordCycle();




        /// <summary>
        /// تعداد کاراکترهایی که هنگام تغییر کلمه عبور میتواند با گلمه عبور قبلی مشترک باشد.
        /// <para>
        /// مقدار پیشفرض 4 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        int? GetPasswordAllowedSameCharacters();


        /// <summary>
        /// چک کردن تکراری نبودن پسورد با دو یا سه پسورد قدیم
        /// <para>
        /// مقدار پیشفرض 3 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        int? GetCountPasswordHistoryRepaed();


        /// <summary>
        /// خواندن مقدار عددی از مقادیر ثابت
        /// </summary>
        /// <param name="Type">نوع مقادیر ثابت مورد نظر</param>
        /// <returns></returns>
        int? GetNumberValue(ConstantType Type);
    }
}