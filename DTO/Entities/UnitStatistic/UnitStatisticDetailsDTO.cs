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

    [Display(Name = "عودت‌کننده")]
    public string ReturnerFullName { get; set; }

    [Display(Name = "تاریخ عودت")]
    public DateTime? ReturnDate { get; set; }

    [Display(Name = "دلیل عودت")]
    public string ReturnReason { get; set; }

    [Display(Name = "لغوکننده")]
    public string CancelerFullName { get; set; }

    [Display(Name = "تاریخ لغو")]
    public DateTime? CancelDate { get; set; }

    [Display(Name = "دلیل لغو")]
    public string CancelReason { get; set; }

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