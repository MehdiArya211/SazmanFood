using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    /// <summary>
    /// پروپرتی های پایه ای که در همه موجودیت ها وجود دارند
    /// </summary>
    public class EntityBase
    {
        [Key]
        [Display(Name = "شناسه")]
        public long Id { get; set; }
        public long? RegUserId { get; set; }
        public long? LastEditUserId { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime? LastEditDate { get; set; }


        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }


        [Display(Name = "حذف شده")]
        public bool? IsDeleted { get; set; }
        

        /// <summary>
        /// constructor
        /// </summary>
        public EntityBase()
        {
            CreateDate = DateTime.Now;
            IsDeleted = false;
        }
    }
}
