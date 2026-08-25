using Domain.Entities.Garrison;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    /// <summary>
    /// پرسنل
    /// </summary>
    public class User : EntityBase
    {
        [Display(Name = "نام کامل")]
        [MaxLength(50, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Name { get; set; }


        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        [Remote("UsernameIsUnique", "CheckUnique", "Admin", AdditionalFields = "Id", HttpMethod = "post", ErrorMessage = "تلفن همراه قبلا استفاده شده است!")]
        [RegularExpression(@"[a-zA-Z0-9@#._\-\*]+", ErrorMessage = "تنها حروف،اعداد و کاراکترهای (- , _ , . , @ , #) مجاز است.")]
        public string Username { get; set; }


        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MinLength(6, ErrorMessage = "{0} باید حداقل {1} کاراکتر باشد.")]
        [MaxLength(150, ErrorMessage = "{0} نمیتواند بیش از {1} کاراکتر باشد.")]
        [RegularExpression(@"[a-zA-Z0-9@#._\-\*]+", ErrorMessage = "تنها حروف انگلیسی، عدد و کاراکتر های (@ ، # ، * ، _ ، - ، .) مجاز است!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "تلفن همراه")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        [RegularExpression(@"^0\d{10}", ErrorMessage = "تلفن با صفر شروع شود و حاوی عدد به طول 11 کاراکتر باشد.")]
        [Remote("MobileIsUnique", "CheckUnique", "Admin", AdditionalFields = "Id", HttpMethod = "post", ErrorMessage = "تلفن همراه قبلا استفاده شده است!")]
        public string Mobile { get; set; }


        [Display(Name = "فعال است")]
        public bool IsEnabled { get; set; }


        [Display(Name = "نقش کاربر")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long RoleId { get; set; }
        public Role Role { get; set; }


        [Display(Name = "نوع کاربر")]
        public UserType Type { get; set; }


        /// <summary>
        /// کاربر هر چند روز باید کلمه عبور خود را عوض کند
        /// </summary>
        [Display(Name = "دوره تغییر کلمه عبور")]
        public int? ChangePasswordCycle { get; set; }


        [Display(Name = "ایجاد کننده")]
        public long? CreatorId { get; set; }
        public User Creator { get; set; }


        /// <summary>
        /// تغییر کلمه عبور از طرف کابر وقتی اولین بار وارد پنل می شود
        /// </summary>
        [Display(Name = "آیا کلمه عبور تغییر کرده")]
        public bool PasswordIsChanged { get; set; }


        [Display(Name = "یگان عمده کاربر")]
        //[Required(ErrorMessage = "{0} الزامی است.")]
        public int OmdOrgId { get; set; }

        [Display(Name = "استان")]
        //[Required(ErrorMessage = "{0} الزامی است.")]
        public int? ProvinceId { get; set; }
        
        [Display(Name = "شهر")]
        //[Required(ErrorMessage = "{0} الزامی است.")]
        public int? CityId { get; set; }


        #region Food Relation

        [Display(Name = "شناسه پرسنل")]
        public long? PersonId { get; set; }

        [Display(Name = "کد پرسنلی")]
        public int? PersonCode { get; set; }

        [Display(Name = "کد ملی")]
        public int? NationalCode { get; set; }

        [Display(Name = "یگان پادگان")]
        public long? OrganGarrisonId { get; set; }

        public Person Person { get; set; }

        #endregion


        #region navigation properties

        #endregion


        public User() : base()
        {
            IsEnabled = true;
            PasswordIsChanged = false;
        }
    }
}
