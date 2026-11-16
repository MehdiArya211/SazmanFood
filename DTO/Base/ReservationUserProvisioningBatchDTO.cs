using System.Collections.Generic;

namespace DTO.Base
{
    /// <summary>
    /// نتیجه هر دسته از ایجاد حساب‌های اولیه برای پرسنل ثبت‌شده در سهمیه.
    /// فراخوانی دسته‌ای برای جلوگیری از timeout در سامانه‌های پرکاربر است.
    /// </summary>
    public sealed class ReservationUserProvisioningBatchDTO
    {
        public long LastId { get; set; }
        public bool HasMore { get; set; }
        public int Examined { get; set; }
        public int Created { get; set; }
        public int AlreadyExists { get; set; }
        public int Failed { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
