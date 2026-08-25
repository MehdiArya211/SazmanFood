using System.ComponentModel.DataAnnotations;
using Utilities.Extentions;

namespace DTO.Entities
{
    public class UnitQuotaPersonDTO
    {
        public long Id { get; set; }
        public long UnitQuotaId { get; set; }
        public long MealId { get; set; }

        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "کد پرسنلی")]
        public string PersonCode { get; set; }

        [Display(Name = "درجه")]
        public string RankTitle { get; set; }

        [Display(Name = "نام و نشان")]
        public string FullName { get; set; }

        [Display(Name = "سالن غذاخوری")]
        public long DiningHallId { get; set; }

        [Display(Name = "سالن غذاخوری")]
        public string DiningHallTitle { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public DateTime CreateDate { get; set; }

        public string CreateDateFa
        {
            get
            {
                return CreateDate
                    .ToPersianDateTime()
                    .ToString();
            }
        }
    }
}