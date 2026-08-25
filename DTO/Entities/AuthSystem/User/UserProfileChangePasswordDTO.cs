using System.ComponentModel.DataAnnotations;

namespace DTO.User
{
    /// <summary>
    /// تغییر کلمه عبور پروفایل کاربران 
    /// </summary>
    public class UserProfileChangePasswordDTO : UserChangePasswordDTO
    {
        [Display(Name = "کلمه عبور فعلی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }


        [Display(Name ="کلمه عبور تغییر کرده؟")]
        public bool PasswordIsChanged { get; set; }

    }
}
