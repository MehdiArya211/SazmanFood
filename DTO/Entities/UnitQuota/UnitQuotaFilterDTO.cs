using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTO.Entities;

public class UnitQuotaFilterDTO
{
    [Display(Name = "یگان")]
    public int? OrgId { get; set; }

    [Display(Name = "نوع سهمیه")]
    public long? YeganTypeId { get; set; }

    [Display(Name = "وعده")]
    public long? MealId { get; set; }

    [Display(Name = "نوع روز")]
    public DayType? DayType { get; set; }
}