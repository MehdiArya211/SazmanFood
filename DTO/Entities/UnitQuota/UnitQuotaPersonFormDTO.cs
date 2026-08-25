using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTO.Entities
{
    public class UnitQuotaPersonFormDTO
    {
        public long UnitQuotaId { get; set; }
        public long UnitStatisticId { get; set; }

        [Display(Name = "یگان")]
        public int OrgId { get; set; }

        [Display(Name = "یگان")]
        public string OrgTitle { get; set; }

        [Display(Name = "نوع سهمیه")]
        public long YeganTypeId { get; set; }

        [Display(Name = "نوع سهمیه")]
        public string YeganTypeTitle { get; set; }

        [Display(Name = "نوع پرسنل")]
        public long PersonalTypeId { get; set; }

        [Display(Name = "نوع پرسنل")]
        public string PersonalTypeTitle { get; set; }

        [Display(Name = "وعده")]
        public long MealId { get; set; }

        [Display(Name = "وعده")]
        public string MealTitle { get; set; }

        [Display(Name = "نوع روز")]
        public DayType DayType { get; set; }

        [Display(Name = "نوع روز")]
        public string DayTypeTitle { get; set; }

        [Display(Name = "تعداد سهمیه")]
        public int QuotaCount { get; set; }

        [Display(Name = "تعداد ثبت‌شده")]
        public int RegisteredCount { get; set; }

        [Display(Name = "ظرفیت باقی‌مانده")]
        public int RemainingCount
        {
            get
            {
                return Math.Max(
                    0,
                    QuotaCount -
                    RegisteredCount);
            }
        }

        public bool IsOfficial { get; set; }
        public bool IsDuty { get; set; }

        public List<UnitQuotaPersonDTO> Persons { get; set; } = new();
    }
}