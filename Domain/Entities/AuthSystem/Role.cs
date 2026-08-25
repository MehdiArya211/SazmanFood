using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Role : EntityBase
    {
        [Display(Name = "عنوان نقش")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }


        /// <summary>
        /// میتوان با غیر فعال کردن یک نقش، موقتا آن را در سایت نشان نداد
        /// </summary>
        [Display(Name = "فعال باشد")]
        public bool IsEnabled { get; set; }
        


        [Display(Name = "توضیحات")]
        [StringLength(300, ErrorMessage = "{0} حداکثر میتواند 300 کاراکتر باشد!")]
        public string? Description { get; set; }



        #region Navigaion Property
        /// <summary>
        /// منوهایی که این نقش به آنها دسترسی دارد
        /// </summary>
        public ICollection<RoleMenu> Menus { get; set; }


        /// <summary>
        /// کاربرانی که این نقش را دارند
        /// </summary>
        public ICollection<User> Users { get; set; }
        #endregion




        public Role() : base()
        {
            IsEnabled = true;
        }

    }
}
