using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTO.Entities
{
    public class UnitCalendarCreateDTO
    {
        [Display(Name = "تاریخ")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public DateTime? CalendarDate { get; set; }

        [Display(Name = "نوع روز")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public DayType? DayType { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(
            500,
            ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
        public string Description { get; set; }
    }
}