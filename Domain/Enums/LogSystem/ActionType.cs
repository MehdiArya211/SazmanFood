using System.ComponentModel;

namespace Domain.Enums
{
    /// <summary>
    /// نوع عملیات
    /// </summary>
    public enum ActionType
    {
        [Description("درج")]
        Create = 0,

        [Description("ویرایش")]
        Update = 1,

        [Description("حذف")]
        Remove = 2,

        [Description("بازدید")]
        View = 3,

        [Description("تغییر رمز عبور")]
        ChangePassword = 4,

        [Description("فعال")]
        Enable = 5,

        [Description("غیرفعال")]
        Disable = 6,

        [Description("موجود")]
        Available = 7,

        [Description("ناموجود")]
        NoAvailable = 8,

        [Description("شروع برنامه")]
        StartApp = 9,

        [Description("پایان برنامه")]
        EndApp = 10,

        [Description("ریست رمز عبور")]
        ResetPassword = 11,

        [Description("ورود")]
        Login = 12,

        [Description("خروج")]
        LogOff = 13,

        [Description("اتمام نشست ورود")]
        CompeleteLoginSession = 14,

        [Description("منبع ناموجود")]
        NonexistentResource = 15,

        [Description("دسترسی غیرمجاز به منبع")]
        UnauthorizedAccessResource = 16,

        [Description("خطای سرور")]
        ServerError = 17,

        [Description("قفل شدن")]
        Lock = 18,

        [Description("خارج شدن از قفل")]
        UnLock = 19,

        [Description("ویرایش دسترسی های نقش به منو")]
        AccessRoleMenu = 20,

        [Description("حذف رابطه")]
        RemoveRelation = 21,

        [Description("تایید")]
        Approve = 22,

        [Description("خواندن")]
        Read = 23,

        [Description("بررسی شده")]
        Checked = 24,

        [Description("بررسی نشده")]
        UnChecked = 25,

        [Description("دانلود اکسل")]
        DownloadExcel = 26,

        [Description("آپلود اکسل")]
        UploadExcel = 27,

        [Description("احراز هویت")]
        Authorize = 28,

    }
}
