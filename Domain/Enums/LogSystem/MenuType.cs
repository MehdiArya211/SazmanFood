using System.ComponentModel;

namespace Domain.Enums
{
    /// <summary>
    /// نوع منو
    /// </summary>
    public enum MenuType
    {
        [Description("ورود کاربر")]
        Login = 0,

        [Description("پروفایل کاربر")]
        Profile = 1,

        [Description("خطای دسترسی")]
        ErrorPage = 2,

        [Description("منو ها")]
        Menus = 3,

        [Description("نقش ها")]
        Roles = 4,

        [Description("پرسنل")]
        Users = 5,

        [Description("لاگ فعالیت")]
        ActionLog = 6,

        [Description("لاگ لاگین")]
        LoginLog = 7,

        [Description("تنظیمات")]
        Constants = 8,

        [Description("فیلتر احراز هویت")]
        AuthorizeFilter = 9,

        [Description("  سهمیه ستاد کل")]
        RateOfSetadKol = 10,

        [Description("  سهمیه  یگانی")]
        RateOfUnit = 11,

        [Description("  مهارت  ")]
        SkillType = 12,

        [Description("  اطلاعات کارگاه  ")]
        WorkShopInf = 13,

        [Description("  اطلاعات دوره  ")]
        Course = 14,

        [Description("  سهمیه استانی  ")]
        RateOfProvince = 15,

        [Description("  سهمیه عمومی  ")]
        RateOfGeneral = 16,

        [Description("  رشته  ")]
        Field = 17,



    }
}
