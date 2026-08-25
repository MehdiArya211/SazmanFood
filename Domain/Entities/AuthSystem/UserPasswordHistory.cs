using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities
{
    /// <summary>
    /// تاریخچه پسورد
    /// </summary>
    public class UserPasswordHistory
    {
        [Key]
        [Display(Name = "شناسه")]
        public long Id { get; set; }

        [Display(Name = "شناسه کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public long UserId { get; set; }

        [Display(Name = "رمز عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Password { get; set; }

        [Display(Name = "تاریخ تغییر رمزعبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public DateTime ChangDate { get; set; }
      
        [Display(Name = "وضعیت ذخیره پسورد توسط مدیر یا کاربر عادی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public bool StatusSave { get; set; }

        [Display(Name = " انقضای پسورد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public bool ExpPassword { get; set; }

    }
}
