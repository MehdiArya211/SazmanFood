using System.ComponentModel.DataAnnotations;

namespace DTO.Entities;

public class UnitStatisticCreateDTO
{
    [Display(Name = "یگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public int OrgId { get; set; }

    [Display(Name = "عنوان یگان")]
    [MaxLength(200, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
    public string OrgTitle { get; set; }

    [Display(Name = "تعداد کل کادر")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [Range(0, int.MaxValue, ErrorMessage = "{0} نمی‌تواند منفی باشد.")]
    public int TotalOfficialCount { get; set; }

    [Display(Name = "تعداد کل وظیفه")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [Range(0, int.MaxValue, ErrorMessage = "{0} نمی‌تواند منفی باشد.")]
    public int TotalDutyCount { get; set; }
}