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

namespace DTO.Entities.FoodMang
{
    public class FoodDTO : EntityDTO
    {
        [Display(Name = "نام غذا")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }

        public long? FoodTypesId { get; set; }
        public string FoodTypeTitle { get; set; }

        [Display(Name = "کد")]
        public int? Code { get; set; }
        public int? MealId { get; set; }

        public static Expression<Func<Foods, FoodDTO>> Selector
        {
            get
            {
                return model => new FoodDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                    FoodTypesId = model.FoodTypesId,
                    Code = model.Code,
                    //MealId = model.FoodTypes,
                    FoodTypeTitle = model.FoodTypes.Title,


                };
            }
        }

    }
}
