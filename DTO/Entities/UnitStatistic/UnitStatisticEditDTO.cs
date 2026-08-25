using Domain.Entities;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

public class UnitStatisticEditDTO
{
    public long Id { get; set; }

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

    public UnitStatisticStatus Status { get; set; }

    public static Expression<Func<UnitStatistic, UnitStatisticEditDTO>> Selector
    {
        get
        {
            return model => new UnitStatisticEditDTO
            {
                Id = model.Id,
                OrgId = model.OrgId,
                OrgTitle = model.OrgTitle,
                TotalOfficialCount = model.TotalOfficialCount,
                TotalDutyCount = model.TotalDutyCount,
                Status = model.Status
            };
        }
    }
}