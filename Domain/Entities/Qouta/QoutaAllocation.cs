using System.ComponentModel.DataAnnotations;
using Domain.Entities.Garrison;

namespace Domain.Entities
{
    /// <summary>
    /// سهمیه بندی غذا برای یگان/قسمت در تاریخ و وعده مشخص
    /// </summary>
    public class QoutaAllocation : EntityBase
    {
        [Display(Name = "یگان پادگان")]
        public long? OrganGarrisonId { get; set; }

        [Display(Name = "برنامه غذایی روز")]
        public long? FoodPlanDayId { get; set; }

        [Display(Name = "یگان")]
        public long? OrgId { get; set; }

        [Display(Name = "نام یگان")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string OrgTitle { get; set; }

        [Display(Name = "تاریخ سهمیه بندی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public DateTime QoutaAllocationDate { get; set; }

        [Display(Name = "شناسه روز")]
        public long DayId { get; set; }

        [Display(Name = "روز")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string DayTitle { get; set; }

        [Display(Name = "شناسه وعده غذایی")]
        public long MealId { get; set; }

        [Display(Name = "وعده غذایی")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string MealTitle { get; set; }

        [Display(Name = "ظرفیت کادر")]
        public int OfficerCapacity { get; set; }

        [Display(Name = "ظرفیت وظیفه")]
        public int SoldierCapacity { get; set; }

        [Display(Name = "ظرفیت مهمان")]
        public int GuestCapacity { get; set; }

        [Display(Name = "ظرفیت ژتون مدیریتی")]
        public int ManagementTokenCapacity { get; set; }

        [Display(Name = "مهلت ثبت غذا")]
        public DateTime? RegisterDeadline { get; set; }

        [Display(Name = "اجازه ثبت توسط کاربر")]
        public bool AllowPersonChange { get; set; }

        [Display(Name = "اجازه ثبت توسط اداری")]
        public bool AllowOfficeUserRegister { get; set; }

        [Display(Name = "وضعیت نهایی")]
        public bool IsFinalized { get; set; }

        #region Relation

        public OrganGarrison OrganGarrison { get; set; }
        public Days Day { get; set; }
        public Meal Meal { get; set; }
        public List<QoutaPerson> QoutaPerson { get; set; }
        public FoodPlanDay FoodPlanDay { get; set; }

        #endregion
    }
}