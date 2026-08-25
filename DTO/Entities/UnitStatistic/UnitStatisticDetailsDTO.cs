using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTO.Entities;

public class UnitStatisticDetailsDTO
{
    public long Id { get; set; }

    [Display(Name = "یگان")]
    public int OrgId { get; set; }

    [Display(Name = "یگان")]
    public string OrgTitle { get; set; }

    [Display(Name = "تعداد کل کادر")]
    public int TotalOfficialCount { get; set; }

    [Display(Name = "تعداد کل وظیفه")]
    public int TotalDutyCount { get; set; }

    [Display(Name = "وضعیت")]
    public UnitStatisticStatus Status { get; set; }

    /// <summary>
    /// جزئیات کادر
    /// </summary>
    public List<UnitStatisticDetailItemDTO>
        OfficialDetails
    { get; set; } = new();

    /// <summary>
    /// جزئیات وظیفه
    /// </summary>
    public List<UnitStatisticDetailItemDTO>
        DutyDetails
    { get; set; } = new();
}