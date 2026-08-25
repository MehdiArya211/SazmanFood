using System.ComponentModel;

namespace Domain.Enums
{
    /// <summary>
    /// وضعیت درخواست
    /// </summary>
    public enum GuestExtraFoodRequestStatus
    {
        [Description("ثبت اولیه")]
        Draft = 1,

        [Description("ارسال شده")]
        Sent = 2,

        [Description("تأیید شده")]
        Approved = 3
    }
}