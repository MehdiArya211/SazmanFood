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
    /// تعریف غذا
    /// </summary>
    public class Foods : EntityBase
    {
        [Display(Name = "نام غذا")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }

        public long? FoodTypesId { get; set; }

        [Display(Name = "کد")]
        public int? Code { get; set; }

        #region Relation
        public FoodTypes FoodTypes { get; set; }


        public List<FoodPlanDay> FoodPlanDay { get; set; }
        //public List<FoodPlanDay> FoodPlanDayDesser { get; set; }
        //public List<FoodPlanDay> FoodPlanDayDorchin { get; set; }

        #endregion
    }
}
