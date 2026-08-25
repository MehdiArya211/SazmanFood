using Domain.Entities.FoodManage;
using DTO.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DTO.Entities.Kitchen
{
    public class KitchenEditDTO:EntityDTO
    {
        [Display(Name = "نام آشپزخانه")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }

        [Display(Name = "ظرفیت پخت")]
        public int? CookingCapacity { get; set; }
        public string CookingCapacitySep { get; set; }

        [Display(Name = "شناسه یگان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int OrgId { get; set; }

        [Display(Name = "یگان")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string OrgTitle { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(500, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "وضعیت")]

        public bool IsActive { get; set; }
        public static Expression<Func<Kitchens, KitchenEditDTO>> Selector
        {
            get
            {
                return model => new KitchenEditDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                    CookingCapacity = model.CookingCapacity,
                    OrgId = model.OrgId,
                    OrgTitle = model.OrgTitle,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    CookingCapacitySep = model.CookingCapacity.HasValue? model.CookingCapacity.Value.ToString("N0") :"0",



                };
            }
        }

    }
}
