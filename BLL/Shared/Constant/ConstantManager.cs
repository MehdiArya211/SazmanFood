using BLL.Interface;
using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using DTO.DataTable;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Utilities.Extentions;

namespace BLL
{
    public class ConstantManager : Manager<Constant, ApplicationContext>, IConstantManager
    {
        public ConstantManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {

        }



        /// <summary>
        /// گرفتن لیست برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<Constant> GetDataTableDTO(DataTableSearchDTO searchData)
        {
            return UOW.Constants.GetDataTableDTO(searchData);
        }




        /// <summary>
        /// ویرایش اطلاعات
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public BaseResult Update(long Id, string Value)
        {
            var constant = GetById(Id);
            var dataType = constant.Type.GetEnumCustomAttribute();

            #region ولیدیت
            if(dataType.Type == CustomDataType.Number)
            {
                int _val;
                if (!int.TryParse(Value, out _val))
                    return new BaseResult(false, "مقدار عددی وارد شده صحیح نیست!");

                if(dataType.Min != 0 && _val < dataType.Min)
                    return new BaseResult(false, $"مقدار وارد شده نباید کمتر از {dataType.Min} باشد!");
                if (dataType.Max != 0 && _val > dataType.Max)
                    return new BaseResult(false, $"مقدار وارد شده نباید بیشتر از {dataType.Max} باشد!");
            }
            else if(dataType.Type == CustomDataType.String)
            {
                if (dataType.Min != 0 && Value.Length < dataType.Min)
                    return new BaseResult(false, $"تعداد کاراکتر وارد شده نباید کمتر از {dataType.Min} باشد!");
                if (dataType.Max != 0 && Value.Length > dataType.Max)
                    return new BaseResult(false, $"تعداد کاراکتر وارد شده نباید بیشتر از {dataType.Max} باشد!");
            }
            #endregion


            #region تبدیل نوع بولین به متن true یا false برای ذخیره
            if (dataType.Type == CustomDataType.Bool)
            {
                if (Value == "on") Value = "true";
                else Value = "false";
            }
            #endregion


            constant.Value = Value;
            return Update(constant);
        }



        /// <summary>
        /// گرفتن تعداد دفعات مجاز برای لاگین ناموفق
        /// <para>
        /// مقدار پیشفرض 5 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int GetFailedLoginCount()
        {
            return UOW.Constants.GetFailedLoginCount();
        }





        /// <summary>
        /// کاربر هر چند روز کلمه عبور را تغییر دهد
        /// <para>
        /// مقدار پیشفرض 60 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int? GetChangePasswordCycle()
        {
            return UOW.Constants.GetFailedLoginCount();
        }




        /// <summary>
        /// تعداد کاراکترهایی که هنگام تغییر کلمه عبور میتواند با گلمه عبور قبلی مشترک باشد.
        /// <para>
        /// مقدار پیشفرض 4 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int? GetPasswordAllowedSameCharacters()
        {
            return UOW.Constants.GetPasswordAllowedSameCharacters();
        }





        /// <summary>
        /// خواندن مقدار عددی از مقادیر ثابت
        /// </summary>
        /// <param name="Type">نوع مقادیر ثابت مورد نظر</param>
        /// <returns></returns>
        public int? GetNumberValue(ConstantType Type)
        {
            return UOW.Constants.GetNumberValue(Type);
        }





    }
}