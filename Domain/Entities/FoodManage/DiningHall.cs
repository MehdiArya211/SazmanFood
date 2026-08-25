using Domain.Enums.Food;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.FoodManage
{
    /// <summary>
    /// سالن غذاخوری
    /// </summary>
    public class DiningHall : EntityBase
    {
        [Display(Name = "نام سالن")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "یگان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int OrgId { get; set; }

        [Display(Name = "عنوان یگان")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
        public string OrgTitle { get; set; }

        [Display(Name = "مسئول سالن")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(150, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
        public string ManagerName { get; set; }

        [Display(Name = "شماره تماس")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(20, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "نوع پرسنل استفاده‌کننده")]
        [Required(ErrorMessage = "{0} الزامی است.")]
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
        public int? HallCapacity { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(500, ErrorMessage = "{0} حداکثر می‌تواند {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }

        #region Relation

        public PersonalType PersonalType { get; set; }

        public Kitchens Kitchen { get; set; }

        #endregion
    }
}