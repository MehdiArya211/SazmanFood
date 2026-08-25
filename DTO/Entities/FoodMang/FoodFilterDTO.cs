using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DTO.Entities.FoodMang
{
    public class FoodFilterDTO
    {
        [Display(Name = "نام غذا")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }

        public long? FoodTypesId { get; set; }

        [Display(Name = "کد")]
        public int? Code { get; set; }
    }
}
