using System.ComponentModel.DataAnnotations;

namespace DTO.Entities
{
    public class UnitQuotaOfficialCreateDTO
    {
        [Required]
        public long UnitQuotaId { get; set; }

        [Display(Name = "وعده")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long MealId { get; set; }

        [Display(Name = "کد پرسنلی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(20, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
        public string PersonCode { get; set; }

        public long? PersonId { get; set; }
        public string NationalCode { get; set; }
        public string RankTitle { get; set; }
        public string FullName { get; set; }

        [Display(Name = "سالن غذاخوری")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long? DiningHallId { get; set; }
    }
}