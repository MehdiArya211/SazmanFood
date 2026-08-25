using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// تقویم اختصاصی یگان
/// </summary>
public class UnitCalendar : EntityBase
{
    [Display(Name = "یگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public int OrgId { get; set; }

    [Display(Name = "عنوان یگان")]
    [MaxLength(200)]
    public string OrgTitle { get; set; }

    [Display(Name = "تاریخ")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public DateTime CalendarDate { get; set; }

    [Display(Name = "نوع روز")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public DayType DayType { get; set; }

    [Display(Name = "توضیحات")]
    [MaxLength(500)]
    public string Description { get; set; }
}