using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Utilities.Extentions;

namespace DTO.User
{
    /// <summary>
    /// لیست کاربران در پنل مدیریت
    /// </summary>
    public class UserDataTableDTO
    {

        [Display(Name = "شناسه")]
        public long Id { get; set; }



        [Display(Name = "نام کاربری")]
        public string Username { get; set; }



        [Display(Name = "نام کامل")]
        public string FullName { get; set; }


        [Display(Name = "تلفن همراه")]
        public string Mobile { get; set; }


        [Display(Name = "وضعیت")]
        public bool IsEnabled { get; set; }


        [Display(Name = "نقش کاربر")]
        public int RoleId { get; set; }
        public string Role { get; set; }


        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "تاریخ ایجاد")]
        public string CreateDateFa { get { return CreateDate.ToPersianDateTime().ToString(); } }


        [Display(Name = "کد پرسنلی")]
        public int? PersonCode { get; set; }

        [Display(Name = "کد ملی")]
        public int? NationalCode { get; set; }

        [Display(Name = "یگان پادگان")]
        public string OrganGarrisonTitle { get; set; }


        /// <summary>
        /// فیلدهای لازم برای استخراج مدل از موجودیت مربوطه
        /// </summary>
        public static Expression<Func<Domain.Entities.User, UserDataTableDTO>> Selector
        {
            get
            {
                return model => new UserDataTableDTO()
                {
                    Id = model.Id,
                    Username = model.Username,
                    FullName = model.Name,
                    Mobile = model.Mobile,
                    Role = model.Role.Title,
                    IsEnabled = model.IsEnabled,
                    CreateDate = model.CreateDate,

                    PersonCode = model.PersonCode,
                    NationalCode = model.NationalCode,
                    OrganGarrisonTitle = model.Person != null && model.Person.OrganGarrison != null
    ? model.Person.OrganGarrison.Title
    : "",
                };
            }
        }
    }
}
