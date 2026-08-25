using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using DTO.DataTable;
using Infrastructure.Data;

namespace BLL.Interface
{
    public interface IConstantManager : IManager<Constant, ApplicationContext>
    {
        /// <summary>
        /// گرفتن لیست برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        DataTableResponseDTO<Constant> GetDataTableDTO(DataTableSearchDTO searchData);



        /// <summary>
        /// ویرایش اطلاعات
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        BaseResult Update(long Id, string Value);





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
        /// خواندن مقدار عددی از مقادیر ثابت
        /// </summary>
        /// <param name="Type">نوع مقادیر ثابت مورد نظر</param>
        /// <returns></returns>
        int? GetNumberValue(ConstantType Type);

    }
}