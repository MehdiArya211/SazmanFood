using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entities
{
    /// <summary>
    /// نوع دسترسی نقش به هر منو
    /// </summary>
    public class RoleMenu
    {
        /// <summary>
        /// کاربر مورد نظر
        /// </summary>
        [Key]
        [Column(Order = 0)]
        [Display(Name = "نقش مورد نظر")]
        public long RoleId { get; set; }
        public Role Role { get; set; }


        /// <summary>
        /// منوای که نقش به آن دسترسی دارد
        /// </summary>
        [Key]
        [Column(Order = 1)]
        [Display(Name = "منو")]
        public long MenuId { get; set; }
        public Menu Menu { get; set; }

    }
}
