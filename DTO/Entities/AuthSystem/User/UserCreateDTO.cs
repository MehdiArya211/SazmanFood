using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DTO.User
{
    /// <summary>
    /// ایجاد پرسنل
    /// </summary>
    public class UserCreateDTO
    {
        [Display(Name = "شناسه")]
        public int Id { get; set; }



        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(50, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string Name { get; set; }


        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [StringLength(30, ErrorMessage = "{0} حداکثر {1} کاراکتر باشد.")]
        [Remote("UsernameIsUnique", "CheckUnique", "Global", AdditionalFields = "Id", HttpMethod = "post", ErrorMessage = "نام کاربری قبلا استفاده شده است!")]
        public string Username { get; set; }



        [Display(Name = "تلفن")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        [RegularExpression(@"^0\d{10}", ErrorMessage = "تلفن با صفر شروع شود و حاوی عدد به طول 11 کاراکتر باشد.")]
        [Remote("MobileIsUnique", "CheckUnique", "Global", AdditionalFields = "Id", HttpMethod = "post", ErrorMessage = "تلفن قبلا استفاده شده است!")]
        public string Mobile { get; set; }



        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MinLength(4, ErrorMessage = "{0} باید حداقل {1} کاراکتر باشد.")]
        [MaxLength(50, ErrorMessage = "{0} نمیتواند بیش از {1} کاراکتر باشد.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,50}$", ErrorMessage = "{0} باید حداقل 8 کاراکتر شامل حروف بزرگ، حروف کوچک لاتین و عدد باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور صحیح نمی باشد.")]
        public string RePassword { get; set; }


        [Display(Name = "دسترسی کاربر")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int RoleId { get; set; }


        [Display(Name = "نوع کاربر")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public UserType Type { get; set; }

        [Display(Name = "یگان عمده کاربر")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int OmdOrgId { get; set; }

        [Display(Name = "استان")]
        //[Required(ErrorMessage = "{0} الزامی است.")]
        public int? ProvinceId { get; set; }

        [Display(Name = "شهر")]
        //[Required(ErrorMessage = "{0} الزامی است.")]
        public int? CityId { get; set; }

        [Display(Name = "پرسنل مرتبط")]
        public long? PersonId { get; set; }

        [Display(Name = "کد پرسنلی")]
        public int? PersonCode { get; set; }

        [Display(Name = "کد ملی")]
        public int? NationalCode { get; set; }

        [Display(Name = "یگان پادگان")]
        public long? OrganGarrisonId { get; set; }

        public int? GharargahId { get; set; }

        public UserCreateDTO()
        {
        }


    }
}
