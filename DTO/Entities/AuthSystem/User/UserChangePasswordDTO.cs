using System.ComponentModel.DataAnnotations;

namespace DTO.User
{
    /// <summary>
    /// تغییر کلمه عبور کاربران 
    /// </summary>
    public class UserChangePasswordDTO
    {
        [Display(Name = "شناسه")]
        public long Id { get; set; }

        [Display(Name = "کلمه عبور جدید")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MinLength(4, ErrorMessage = "{0} باید حداقل {1} کاراکتر باشد.")]
        [MaxLength(50, ErrorMessage = "{0} نمیتواند بیش از {1} کاراکتر باشد.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@%*#?&]).{8,50}$", ErrorMessage = "{0} باید حداقل 8 کاراکتر شامل حروف بزرگ و کوچک لاتین, عدد و یکی از کاراکترهای !@%*#?& باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "تکرار کلمه عبور جدید")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور صحیح نمی باشد.")]
        public string RePassword { get; set; }

    }
}
