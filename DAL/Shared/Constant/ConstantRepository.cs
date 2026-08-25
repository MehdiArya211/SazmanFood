using DAL.Interface;
using Domain.Entities;
using Domain.Enums;
using DTO.DataTable;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Utilities.Extentions;
using Z.EntityFramework.Plus;

namespace DAL
{
    public class ConstantRepository : Repository<Constant>, IConstantRepository
    {
        public ConstantRepository(DbContext _Context) : base(_Context)
        {
        }




        /// <summary>
        /// گرفتن لیست اندازه ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<Constant> GetDataTableDTO(DataTableSearchDTO searchData)
        {
            var model = new DataTableResponseDTO<Constant>();

            var recordTotal = Entities.DeferredCount().FutureValue();

            var filter = PredicateBuilder.New<Constant>(true);

            #region شرط ها

            //search
            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue;
                filter = filter.And(s => s.Title.Contains(srch)
                                              || s.Title.Contains(srch)
                                              || s.Id.ToString().Contains(srch));
            }
            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            //sorting and paging
            var sortCol = searchData.sortColumnName;
            var selectedModel = Entities.Where(filter)
                                        .OrderBy(sortCol + " " + searchData.sortDirection)
                                        .Skip(searchData.start)
                                        .Take(searchData.length)
                                        .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
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
            return GetNumberValue(ConstantType.FailedLoginCount) ?? 5;
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
            return GetNumberValue(ConstantType.ChangePasswordCycle);
        }

        /// <summary>
        /// چک کردن تکراری نبودن پسورد با دو یا سه پسورد قدیم
        /// <para>
        /// مقدار پیشفرض 3 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int? GetCountPasswordHistoryRepaed()
        {
            return GetNumberValue(ConstantType.PasswordRepaedByOldPass);
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
            return GetNumberValue(ConstantType.PasswordAllowedSameCharacters);
        }





        /// <summary>
        /// خواندن مقدار عددی از مقادیر ثابت
        /// </summary>
        /// <param name="Type">نوع مقادیر ثابت مورد نظر</param>
        /// <returns></returns>
        public int? GetNumberValue(ConstantType Type)
        {
            try
            {
                #region نوع پارامتر عددی باشد
                var dataType = Type.GetEnumCustomAttribute();
                if (dataType.Type != CustomDataType.Number)
                    return null;
                #endregion

                #region گرفتن مقدار پیشفرض
                int? defaultValue = null;
                int _val;
                if (!string.IsNullOrEmpty(dataType.DefultValue))
                    if (int.TryParse(dataType.DefultValue, out _val))
                        defaultValue = _val;
                #endregion

                #region گرفتن مقدار و اعتبار سنجی
                var value = GetOneDTO(x => x.Value, x => x.Type == Type);
                int intValue;
                if (string.IsNullOrEmpty(value) || !int.TryParse(value, out intValue))
                    return defaultValue;

                if (intValue < dataType.Min || intValue > dataType.Max)
                    return defaultValue;

                return intValue;
                #endregion
            }
            catch
            {
                return null;
            }

        }




    }
}