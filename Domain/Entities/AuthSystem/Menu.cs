using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    /// <summary>
    /// منو های سایت
    /// </summary>
    public class Menu : EntityBase
    {
        [Display(Name = "عنوان منو")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }
        

        /// <summary>
        /// شناسه منو سر دسته
        /// </summary>
        [Display(Name = "سر دسته")]
        public long? ParentId { get; set; }
        public Menu Parent { get; set; }



        /// <summary>
        /// ترتیب نمایش منو ها
        /// </summary>
        [Display(Name = "ترتیب")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int Sort { get; set; }


        [Display(Name = "عنوان آیکن متریال")]
        public string MaterialIcon { get; set; }

        
        /// <summary>
        /// در لیست منوها نمایش داده شود یا صرفا
        /// یک اکشن است که دسترسی برای آن تعیین می شود
        /// </summary>
        [Display(Name = "نمایش در منو")]
        public bool ShowInMenu { get; set; }


        /// <summary>
        /// میتوان با غیر فعال کردن یک منو، موقتا آن را در سایت نشان نداد
        /// و از دسترسی کاربران به آن منو جلوگیری کرد
        /// </summary>
        [Display(Name = "فعال باشد")]
        public bool IsEnabled { get; set; }


        /// <summary>
        /// آیا منو درج شده لینک دارد یا خیر.
        /// منو های سر دسته لینک ندارند
        /// </summary>
        [Display(Name = "لینک دارد")]
        public bool HasLink { get; set; }



        /// <summary>
        /// آیا دسترسی به منو مربوطه نیاز به احراز هویت مجدد دارد؟
        /// </summary>
        [Display(Name = "نیاز به احراز هویت مجدد دارد؟")]
        public bool NeedReAuthorize { get; set; }



        #region پروپرتی های مربوط به ایجاد لینک منو
        /// <summary>
        /// نام اریایی که با کلیک بر روی منو باید اجرا شود
        /// </summary>
        [Display(Name = "اریا")]
        public string Area { get; set; }


        /// <summary>
        /// نام کنترلری که با کلیک بر روی منو باید اجرا شود
        /// </summary>
        [Display(Name = "کنترلر")]
        public string Controller { get; set; }


        /// <summary>
        /// نام اکشنی که با کلیک بر روی منو باید اجرا شود
        /// </summary>
        [Display(Name = "اکشن")]
        public string Action { get; set; }


        /// <summary>
        /// پارامترهای آدرس که به صورت جیسون از کاربر دریافت می شود
        /// و پس از تبدیل به رشته ذخیره می گردد
        /// </summary>
        [Display(Name = "پارامترهای آدرس")]
        public string Parameters { get; set; }
        #endregion

        

        [Display(Name = "توضیحات")]
        [StringLength(300, ErrorMessage = "{0} حداکثر میتواند 300 کاراکتر باشد!")]
        public string Description { get; set; }



        #region Navigaion Property
        /// <summary>
        /// منوهای زیر دسته
        /// </summary>
        public ICollection<Menu> SubMenus { get; set; }


        /// <summary>
        /// نقش هایی که به این منو دسترسی دارند
        /// </summary>
        public ICollection<RoleMenu> Roles { get; set; }
        #endregion



        public Menu() : base()
        {
            HasLink = true;
            IsEnabled = true;
            ShowInMenu = true;
            Sort = 1;
        }


    }
}
