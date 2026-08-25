using System.ComponentModel;

namespace Domain.Enums
{
    public enum UnitStatisticStatus
    {
        [Description("ثبت اولیه")]
        Draft = 1,

        [Description("ارسال‌شده")]
        Sent = 2,

        [Description("تأییدشده")]
        Approved = 3
    }
}