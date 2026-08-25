using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DTO.Entities.FoodMang
{
    public class FoodsCreateDTO
    {
        [Display(Name = "نام غذا")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }

        [Display(Name = "نوع غذا")]

        public long? FoodTypesId { get; set; }
        public string FoodTypeTitle { get; set; }

        [Display(Name = "کد")]
        public int? Code { get; set; }
    }
}
