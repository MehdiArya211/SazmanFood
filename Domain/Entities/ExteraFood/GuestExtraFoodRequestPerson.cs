using System.ComponentModel.DataAnnotations;
using Domain.Entities.FoodManage;

namespace Domain.Entities
{
    public class GuestExtraFoodRequestPerson : EntityBase
    {
        [Display(Name = "درخواست غذای مهمان / مازاد")]
        [Required]
        public long GuestExtraFoodRequestId { get; set; }

        [Display(Name = "یگان")]
        [Required]
        public int OrgId { get; set; }

        [Display(Name = "وعده")]
        [Required]
        public long MealId { get; set; }

        [Display(Name = "نوع پرسنل")]
        [Required]
        public long PersonalTypeId { get; set; }

        [Display(Name = "سالن غذاخوری")]
        public long? DiningHallId { get; set; }

        [Display(Name = "شناسه پرسنل")]
        public long? PersonId { get; set; }

        [Display(Name = "کد پرسنلی")]
        [MaxLength(20)]
        public string PersonCode { get; set; }

        [Display(Name = "کد ملی")]
        [MaxLength(10)]
        public string NationalCode { get; set; }

        [Display(Name = "درجه")]
        [MaxLength(100)]
        public string RankTitle { get; set; }

        [Display(Name = "نام و نشان")]
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Display(Name = "ثبت‌کننده")]
        public long CreatorId { get; set; }

        [Display(Name = "ثبت‌کننده")]
        [MaxLength(200)]
        public string CreatorFullName { get; set; }

        public GuestExtraFoodRequest GuestExtraFoodRequest { get; set; }

        public DiningHall DiningHall { get; set; }
    }
}