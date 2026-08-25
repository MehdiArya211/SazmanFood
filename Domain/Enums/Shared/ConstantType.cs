using System.Collections.Generic;
using System.ComponentModel;
using Utilities.Extentions;

namespace Domain.Enums
{
    /// <summary>
    /// نوع پارامتر موجود در تیبل 
    /// </summary>
    public enum ConstantType
    {
        [CustomDataType(Type = CustomDataType.String, Max = 300)]
        [Description("تلفن")]
        Tel = 0,


        [CustomDataType(Type = CustomDataType.String, Max = 300)]
        [Description("ایمیل")]
        Email = 1,


        [CustomDataType(Type = CustomDataType.String, Max = 300)]
        [Description("آدرس")]
        Address = 2,


        /// <summary>
        /// بعد از چند بار تلاش ناموفق حساب کاربری قفل شود؟
        /// </summary>
        [CustomDataType(Type = CustomDataType.Number, Min = 1, Max = 10, Placeholder = "بین 1 تا 10", DefultValue = "5")]
        [Description("تعداد دفعات مجاز برای ورود ناموفق")]
        FailedLoginCount = 3,



        /// <summary>
        /// کاربر هر چند روز باید کلمه عبور خود را عوض کند
        /// </summary>
        [CustomDataType(Type = CustomDataType.Number, Min = 0, Placeholder = "کاربر هر چند روز باید کلمه عبور خود را تغییر دهد.", DefultValue = "60")]
        [Description("اجبار تغییر کلمه عبور کاربر بعد از چند روز؟")]
        ChangePasswordCycle = 4,



        /// <summary>
        /// تعداد کاراکترهایی که هنگام تغییر کلمه عبور میتواند با گلمه عبور قبلی مشترک باشد.
        /// </summary>
        [CustomDataType(Type = CustomDataType.Number, Min = 0, Placeholder = "کلمه عبور جدید می تواند حداکثر چند کاراکتر مشترک با کلمه عبور قبلی داشته باشد؟", DefultValue = "4")]
        [Description("تعداد کاراکتر های تکراری مجاز در تغییر کلمه عبور")]
        PasswordAllowedSameCharacters = 5,

        /// <summary>
        /// چک کردن تکراری نبودن پسورد با دو یا سه پسورد قدیم
        /// </summary>
        [CustomDataType(Type = CustomDataType.Number, Min = 0, Placeholder = "چک کردن تکراری نبودن پسورد با دو یا سه پسورد قدیم", DefultValue = "3")]
        [Description("چک کردن تکراری نبودن پسورد با دو یا سه پسورد قدیم")]
        PasswordRepaedByOldPass = 6,

    }




}
