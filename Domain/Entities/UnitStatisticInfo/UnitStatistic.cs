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

    /// <summary>
    /// فقط آخرین آمار تأییدشده یگان فعال است.
    /// </summary>
    public bool IsActive { get; set; }

    #region Relation

    public List<UnitStatisticDetail> Details { get; set; }

    public List<UnitQuota> Quotas { get; set; }

    #endregion
}