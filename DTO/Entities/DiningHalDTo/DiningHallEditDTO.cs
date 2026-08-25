using Domain.Entities.FoodManage;
using Domain.Enums.Food;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace DTO.Entities;

public class DiningHallEditDTO
{
    public long Id { get; set; }

    [Display(Name = "نام سالن")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [MaxLength(200, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
    public string Title { get; set; }

    [Display(Name = "یگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [Range(1, int.MaxValue, ErrorMessage = "{0} را انتخاب کنید.")]
    public int OrgId { get; set; }

    [Display(Name = "عنوان یگان")]
    [MaxLength(200)]
    public string OrgTitle { get; set; }

    [Display(Name = "مسئول سالن")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [MaxLength(150, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
    public string ManagerName { get; set; }

    [Display(Name = "شماره تماس")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [MaxLength(20, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
    [RegularExpression(
        @"^[0-9۰-۹٠-٩+\-\s]+$",
        ErrorMessage = "فرمت شماره تماس صحیح نیست.")]
    public string PhoneNumber { get; set; }

    [Display(Name = "نوع پرسنل استفاده‌کننده")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [Range(1, long.MaxValue, ErrorMessage = "{0} را انتخاب کنید.")]
    public long PersonalTypeId { get; set; }

    [Display(Name = "نوع استفاده")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public DiningHallUsageType UsageType { get; set; }

    [Display(Name = "محل تهیه غذا")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public DiningHallSupplyType SupplyType { get; set; }

    [Display(Name = "آشپزخانه")]
    public long? KitchenId { get; set; }

    [Display(Name = "ظرفیت سالن")]
    [Range(1, int.MaxValue, ErrorMessage = "{0} باید بیشتر از صفر باشد.")]
    public int? HallCapacity { get; set; }

    public string HallCapacitySep { get; set; }

    [Display(Name = "توضیحات")]
    [MaxLength(500, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
    public string Description { get; set; }

    [Display(Name = "وضعیت")]
    public bool IsActive { get; set; }

    public bool IsInternal { get; set; }


    public static Expression<Func<DiningHall, DiningHallEditDTO>>
        Selector
    {
        get
        {
            return model => new DiningHallEditDTO
            {
                Id = model.Id,
                Title = model.Title,

                OrgId = model.OrgId,
                OrgTitle = model.OrgTitle,

                ManagerName = model.ManagerName,
                PhoneNumber = model.PhoneNumber,

                PersonalTypeId = model.PersonalTypeId,
                UsageType = model.UsageType,
                SupplyType = model.SupplyType,
                KitchenId = model.KitchenId,

                HallCapacity = model.HallCapacity,
                Description = model.Description,
                IsActive = model.IsActive
            };
        }
    }
}