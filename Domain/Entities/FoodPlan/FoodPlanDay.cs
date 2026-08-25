using Domain.Entities.FoodManage;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// برنامه غذایی ایام هفته
    /// </summary>
    public class FoodPlanDay : EntityBase
    {

        [Display(Name = "شناسه برنامه غذایی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long FoodPlanId { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long DayId { get; set; }
        
        [Display(Name = "وعده غذایی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long MealId { get; set; }

        [Display(Name = "غذا")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long FoodId { get; set; }

        [Display(Name = "دسر")]
        public long? FoodDesserId { get; set; }

        [Display(Name = "دورچین")]
        public long? FoodDorchinId { get; set; }


        [Display(Name = "تعداد")]
        public int? Count { get; set; }


        #region relation
        public Foods Food { get; set; }

        public FoodPlan FoodPlan { get; set; }
        public Meal Meal { get; set; }
        public Days Day { get; set; }
        public List<QoutaAllocation> QoutaAllocations { get; set; }


        #endregion

    }

}
