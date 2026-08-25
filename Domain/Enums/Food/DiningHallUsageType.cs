using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Food
{
    /// <summary>
    /// نوع استفاده از سالن غذاخوری
    /// </summary>
    public enum DiningHallUsageType
    {
        [Display(Name = "عمومی")]
        Public = 2,

        [Display(Name = "مختص یگان")]
        OrganizationOnly = 1
    }

    /// <summary>
    /// محل تهیه غذای سالن
    /// </summary>
    public enum DiningHallSupplyType
    {
        [Display(Name = "آشپزخانه مرکزی")]
        CentralKitchen = 1,

        [Display(Name = "داخل یگان")]
        InternalOrganization = 2
    }
}