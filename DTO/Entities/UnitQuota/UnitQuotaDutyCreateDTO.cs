using System.ComponentModel.DataAnnotations;

namespace DTO.Entities
{
    public class UnitQuotaDutyCreateDTO
    {
        [Required]
        public long UnitQuotaId { get; set; }

        [Display(Name = "وعده")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long MealId { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} باید 10 رقم باشد.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "{0} باید شامل 10 رقم باشد.")]
        public string NationalCode { get; set; }

        [Display(Name = "کد پرسنلی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(20, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
        public string PersonCode { get; set; }

        [Display(Name = "نام و نشان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
        public string FullName { get; set; }

        [Display(Name = "سالن غذاخوری")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long? DiningHallId { get; set; }
    }
}