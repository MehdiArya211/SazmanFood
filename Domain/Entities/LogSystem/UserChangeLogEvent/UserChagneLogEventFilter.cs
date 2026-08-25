using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DTO.Entities.LogSystem.UserChangeLogEvent;

public class UserChagneLogEventFilter
{
    [Display(Name = "نوع عملیات")]
    public ChangeEventType ChangeEventType { get; set; }
    [Display(Name = "نام جدول")]
    public string TableName { get; set; }

    public long? TableId { get; set; }

    public long UserId { get; set; }

    public int? UserOrgId { get; set; }

    [Display(Name = "نام کاربر")]
    public string UserFullName { get; set; }

    [Display(Name = "کدپرسنلی")]
    public string UserName { get; set; }
    [Display(Name = "آی پی")]
    public string IpAddress { get; set; }


    [Display(Name = "ایجاد از تاریخ")]
    public DateTime? CreateStartDate { get; set; }


    [Display(Name = "ایجاد تا تاریخ")]
    public DateTime? CreateEndDate { get; set; }
}
