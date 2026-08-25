using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class FoodSource : EntityBase
    {
        [Display(Name = "نوع خدمت")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long YeganTypeId { get; set; }

        [Display(Name = "نوع پرسنل")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long PersonalTypeId { get; set; }

        [Display(Name = "نوع روز")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public DayType DayType { get; set; } = DayType.Normal;

        [Display(Name = "درصد صبحانه")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [Range(0, 100, ErrorMessage = "{0} باید بین صفر تا صد باشد.")]
        public double PercentBreakfast { get; set; }

        [Display(Name = "درصد ناهار")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [Range(0, 100, ErrorMessage = "{0} باید بین صفر تا صد باشد.")]
        public double PercentLunch { get; set; }

        [Display(Name = "درصد شام")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [Range(0, 100, ErrorMessage = "{0} باید بین صفر تا صد باشد.")]
        public double PercentDinner { get; set; }

        public YeganType YeganType { get; set; }
        public PersonalType PersonalType { get; set; }
        public List<UnitQuota> UnitQuotas { get; set; }
    }
}