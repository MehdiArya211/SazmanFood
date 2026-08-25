using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace DTO.User
{
    /// <summary>
    /// ویرایش پروفایل توسط کاربر
    /// </summary>
    public class UserEditProfileDTO
    {
        [Display(Name = "شناسه")]
        public long Id { get; set; }


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


        /// <summary>
        /// کاربر هر چند روز باید کلمه عبور خود را عوض کند
        /// </summary>
        [Display(Name = "دوره تغییر کلمه عبور")]
        public int? ChangePasswordCycle { get; set; }



        /// <summary>
        /// فیلدهای لازم برای استخراج مدل از موجودیت مربوطه
        /// </summary>
        public static Expression<Func<Domain.Entities.User, UserEditProfileDTO>> Selector
        {
            get
            {
                return model => new UserEditProfileDTO()
                {
                    Id = model.Id,
                    Mobile = model.Mobile,
                    Name = model.Name,
                    Username = model.Username,
                    ChangePasswordCycle = model.ChangePasswordCycle
                };
            }
        }


    }
}
