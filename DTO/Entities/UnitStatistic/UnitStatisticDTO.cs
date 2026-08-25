using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Utilities.Extentions;

namespace DTO.Entities;

public class UnitStatisticDTO
{
    public long Id { get; set; }

    [Display(Name = "یگان")]
    public int OrgId { get; set; }

    [Display(Name = "عنوان یگان")]
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
    public string CreatorFullName { get; set; }

    [Display(Name = "تاریخ ثبت")]
    public DateTime CreateDate { get; set; }

    public string CreateDateFa
    {
        get
        {
            return CreateDate.ToPersianDateTime().ToString();
        }
    }

    [Display(Name = "تأییدکننده")]
    public long? ApproverId { get; set; }

    [Display(Name = "تأییدکننده")]
    public string ApproverFullName { get; set; }

    [Display(Name = "تاریخ تأیید")]
    public DateTime? ApproveDate { get; set; }

    public string ApproveDateFa
    {
        get
        {
            return ApproveDate.HasValue
                ? ApproveDate.Value.ToPersianDateTime().ToString()
                : "-";
        }
    }

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

    [Display(Name = "فعال")]
    public bool IsActive { get; set; }

    public string StatusTitle
    {
        get
        {
            return Status switch
            {
                UnitStatisticStatus.Draft => "ثبت اولیه",
                UnitStatisticStatus.Sent => "ارسال شده",
                UnitStatisticStatus.Approved => "تأیید نهایی",
                UnitStatisticStatus.Returned => "عودت‌شده",
                UnitStatisticStatus.Canceled => "لغوشده",
                _ => "نامشخص"
            };
        }
    }

    public static Expression<Func<UnitStatistic, UnitStatisticDTO>> Selector
    {
        get
        {
            return model => new UnitStatisticDTO
            {
                Id = model.Id,
                OrgId = model.OrgId,
                OrgTitle = model.OrgTitle,
                TotalOfficialCount = model.TotalOfficialCount,
                TotalDutyCount = model.TotalDutyCount,
                Status = model.Status,
                CreatorId = model.CreatorId,
                CreatorFullName = model.CreatorFullName,
                CreateDate = model.CreateDate,
                ApproverId = model.ApproverId,
                ApproverFullName = model.ApproverFullName,
                ApproveDate = model.ApproveDate,
                ReturnerFullName = model.ReturnerFullName,
                ReturnDate = model.ReturnDate,
                ReturnReason = model.ReturnReason,
                CancelerFullName = model.CancelerFullName,
                CancelDate = model.CancelDate,
                CancelReason = model.CancelReason,
                IsActive = model.IsActive
            };
        }
    }
}
