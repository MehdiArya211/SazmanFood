using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// سهمیه محاسبه‌شده یگان
    /// </summary>
    public class UnitQuota : EntityBase
    {
        [Display(Name = "آمار یگان")]
        public long UnitStatisticId { get; set; }

        [Display(Name = "یگان")]
        public int OrgId { get; set; }

        [Display(Name = "عنوان یگان")]
        [MaxLength(200)]
        public string OrgTitle { get; set; }

        [Display(Name = "نوع پرسنل")]
        public long PersonalTypeId { get; set; }

        [Display(Name = "نوع خدمت")]
        public long YeganTypeId { get; set; }

        [Display(Name = "نوع روز")]
        public DayType DayType { get; set; }

        [Display(Name = "مأخذ غذایی")]
        public long FoodSourceId { get; set; }

        [Display(Name = "تعداد پرسنل")]
        public int PersonnelCount { get; set; }

        [Display(Name = "درصد صبحانه")]
        public double PercentBreakfast { get; set; }

        [Display(Name = "سهمیه صبحانه")]
        public int BreakfastQuota { get; set; }

        [Display(Name = "درصد ناهار")]
        public double PercentLunch { get; set; }

        [Display(Name = "سهمیه ناهار")]
        public int LunchQuota { get; set; }

        [Display(Name = "درصد شام")]
        public double PercentDinner { get; set; }

        [Display(Name = "سهمیه شام")]
        public int DinnerQuota { get; set; }

        [Display(Name = "تاریخ محاسبه")]
        public DateTime CalculateDate { get; set; }

        #region Relation

        public UnitStatistic UnitStatistic { get; set; }
        public PersonalType PersonalType { get; set; }
        public YeganType YeganType { get; set; }
        public FoodSource FoodSource { get; set; }
        public List<UnitQuotaPerson> Persons { get; set; } = new();

        #endregion
    }
}