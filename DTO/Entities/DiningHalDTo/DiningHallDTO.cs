using Domain.Entities.FoodManage;
using Domain.Enums.Food;
using DTO.Base;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace DTO.Entities;

public class DiningHallDTO : EntityDTO
{
    [Display(Name = "نام سالن")]
    public string Title { get; set; }

    [Display(Name = "یگان")]
    public int OrgId { get; set; }

    [Display(Name = "یگان")]
    public string OrgTitle { get; set; }

    [Display(Name = "مسئول سالن")]
    public string ManagerName { get; set; }

    [Display(Name = "شماره تماس")]
    public string PhoneNumber { get; set; }

    [Display(Name = "نوع پرسنل")]
    public long PersonalTypeId { get; set; }

    public string PersonalTypeTitle { get; set; }

    [Display(Name = "نوع استفاده")]
    public DiningHallUsageType UsageType { get; set; }

    public string UsageTypeTitle { get; set; }

    [Display(Name = "محل تهیه غذا")]
    public DiningHallSupplyType SupplyType { get; set; }

    public string SupplyTypeTitle { get; set; }

    [Display(Name = "آشپزخانه")]
    public long? KitchenId { get; set; }

    public string KitchenTitle { get; set; }

    [Display(Name = "ظرفیت سالن")]
    public int? HallCapacity { get; set; }

    public string HallCapacityStr
    {
        get
        {
            return HallCapacity.HasValue
                ? HallCapacity.Value.ToString("N0")
                : "0";
        }
    }

    [Display(Name = "توضیحات")]
    public string Description { get; set; }

    [Display(Name = "وضعیت")]
    public bool IsActive { get; set; }

    public static Expression<Func<DiningHall, DiningHallDTO>>
        Selector
    {
        get
        {
            return model => new DiningHallDTO
            {
                Id = model.Id,
                Title = model.Title,

                OrgId = model.OrgId,
                OrgTitle = model.OrgTitle,

                ManagerName = model.ManagerName,
                PhoneNumber = model.PhoneNumber,

                PersonalTypeId = model.PersonalTypeId,
                PersonalTypeTitle = model.PersonalType.Title,

                UsageType = model.UsageType,

                UsageTypeTitle =
                    model.UsageType ==
                    DiningHallUsageType.Public
                        ? "عمومی"
                        : "مختص یگان",

                SupplyType = model.SupplyType,

                SupplyTypeTitle =
                    model.SupplyType ==
                    DiningHallSupplyType.InternalOrganization
                        ? "داخل یگان"
                        : "آشپزخانه مرکزی",

                KitchenId = model.KitchenId,

                KitchenTitle = model.Kitchen != null
                    ? model.Kitchen.Title
                    : "-",

                HallCapacity = model.HallCapacity,
                Description = model.Description,
                IsActive = model.IsActive
            };
        }
    }
}