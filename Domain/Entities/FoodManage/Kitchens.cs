using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities.FoodManage
{
    /// <summary>
    /// تعریف آشپزخانه
    /// </summary>
    public class Kitchens : EntityBase
    {
        [Display(Name = "نام آشپزخانه")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }

        [Display(Name = "ظرفیت پخت")]
        public int? CookingCapacity { get; set; }

        [Display(Name = "شناسه یگان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int OrgId { get; set; }

        [Display(Name = "یگان")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string OrgTitle { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(500, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public List<DiningHall> DiningHalls { get; set; }

    }
}
