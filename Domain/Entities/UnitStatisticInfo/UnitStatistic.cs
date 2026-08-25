using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// آمار یگان
/// </summary>
public class UnitStatistic : EntityBase
{
    [Display(Name = "یگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public int OrgId { get; set; }

    [Display(Name = "عنوان یگان")]
    [MaxLength(200)]
    public string OrgTitle { get; set; }

    [Display(Name = "تعداد کل کادر")]
    public int TotalOfficialCount { get; set; }

    [Display(Name = "تعداد کل وظیفه")]
    public int TotalDutyCount { get; set; }

    [Display(Name = "وضعیت آمار")]
    public UnitStatisticStatus Status { get; set; }

    [Display(Name = "ثبت‌کننده")]
    public long CreatorId { get; set; }

    [Display(Name = "ثبت‌کننده")]
    [MaxLength(200)]
    public string CreatorFullName { get; set; }

    [Display(Name = "تاریخ ثبت")]
    public DateTime CreateDate { get; set; }

    [Display(Name = "تأییدکننده")]
    public long? ApproverId { get; set; }

    [Display(Name = "تأییدکننده")]
    [MaxLength(200)]
    public string ApproverFullName { get; set; }

    [Display(Name = "تاریخ تأیید")]
    public DateTime? ApproveDate { get; set; }

    [Display(Name = "عودت‌کننده")]
    public long? ReturnerId { get; set; }

    [Display(Name = "عودت‌کننده")]
    [MaxLength(200)]
    public string ReturnerFullName { get; set; }

    [Display(Name = "تاریخ عودت")]
    public DateTime? ReturnDate { get; set; }

    [Display(Name = "دلیل عودت")]
    [MaxLength(1000)]
    public string ReturnReason { get; set; }

    [Display(Name = "لغوکننده")]
    public long? CancelerId { get; set; }

    [Display(Name = "لغوکننده")]
    [MaxLength(200)]
    public string CancelerFullName { get; set; }

    [Display(Name = "تاریخ لغو")]
    public DateTime? CancelDate { get; set; }

    [Display(Name = "دلیل لغو")]
    [MaxLength(1000)]
    public string CancelReason { get; set; }

    /// <summary>
    /// فقط آخرین آمار تأییدشده یگان فعال است.
    /// </summary>
    public bool IsActive { get; set; }

    #region Relation

    public List<UnitStatisticDetail> Details { get; set; }

    public List<UnitQuota> Quotas { get; set; }

    #endregion
}