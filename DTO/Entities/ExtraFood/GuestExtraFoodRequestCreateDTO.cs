using System.ComponentModel.DataAnnotations;

namespace DTO.Entities
{
    public class GuestExtraFoodRequestCreateDTO
    {
        [Display(Name = "یگان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int? OrgId { get; set; }

        [Display(Name = "وعده")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long? MealId { get; set; }

        [Display(Name = "نوع پرسنل")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long? PersonalTypeId { get; set; }

        [Display(Name = "نوع خدمت")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long? YeganTypeId { get; set; }

        [Display(Name = "از تاریخ")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "تا تاریخ")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "تعداد مهمان")]
        [Range(0, int.MaxValue)]
        public int GuestCount { get; set; }

        [Display(Name = "تعداد مازاد بر سهمیه")]
        [Range(0, int.MaxValue)]
        public int ExtraQuotaCount { get; set; }
    }

    public class GuestExtraFoodRequestEditDTO
        : GuestExtraFoodRequestCreateDTO
    {
        [Required]
        public long Id { get; set; }
    }
}