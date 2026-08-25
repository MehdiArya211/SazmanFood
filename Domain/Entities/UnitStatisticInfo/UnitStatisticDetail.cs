using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// جزئیات آمار یگان
/// </summary>
public class UnitStatisticDetail : EntityBase
{
    [Display(Name = "آمار یگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long UnitStatisticId { get; set; }

    [Display(Name = "نوع پرسنل")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long PersonalTypeId { get; set; }

    [Display(Name = "نوع خدمت")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long YeganTypeId { get; set; }

    [Display(Name = "تعداد")]
    [Range(0, int.MaxValue, ErrorMessage = "{0} نمی‌تواند منفی باشد.")]
    public int Count { get; set; }

    #region Relation

    public UnitStatistic UnitStatistic { get; set; }

    public PersonalType PersonalType { get; set; }

    public YeganType YeganType { get; set; }

    #endregion
}