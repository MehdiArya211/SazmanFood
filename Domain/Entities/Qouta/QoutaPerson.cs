using System.ComponentModel.DataAnnotations;
using Domain.Entities.FoodManage;
using Domain.Entities.Garrison;

namespace Domain.Entities
{
    /// <summary>
    /// نفرات تخصیص یافته به سهمیه غذا
    /// </summary>
    public class QoutaPerson : EntityBase
    {
        [Display(Name = "شناسه سهمیه بندی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long QoutaAllocationId { get; set; }

        [Display(Name = "نوع دریافت کننده")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long FoodReceiverTypeId { get; set; }

        [Display(Name = "نوع ژتون")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long FoodTokenTypeId { get; set; }

        [Display(Name = "شناسه پرسنل")]
        public long? PersonId { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string FName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string LName { get; set; }

        [Display(Name = "کد پرسنلی / کد مهمان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string PersonalCode { get; set; }

        [Display(Name = "نوع پرسنل")]
        public long? PersonalTypeId { get; set; }

        [Display(Name = "غذای اصلی")]
        public long? MainFoodId { get; set; }

        [Display(Name = "دسر")]
        public long? DessertId { get; set; }

        [Display(Name = "دورچین")]
        public long? SideDishId { get; set; }

        [Display(Name = "یگان")]
        public long? OrgId { get; set; }

        [Display(Name = "نام یگان")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string OrgTitle { get; set; }

        [Display(Name = "یگان پادگان")]
        public long? OrganGarrisonId { get; set; }

        [Display(Name = "کد تحویل")]
        [MaxLength(100, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string DeliveryCode { get; set; }

        [Display(Name = "کد امنیتی تحویل")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string DeliveryHash { get; set; }

        [Display(Name = "تحویل شده")]
        public bool IsDelivered { get; set; }

        [Display(Name = "زمان تحویل")]
        public DateTime? DeliveredDate { get; set; }

        [Display(Name = "شناسه کاربر تحویل دهنده")]
        public long? DeliveredUserId { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(500, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string Description { get; set; }

        #region Relation

        public QoutaAllocation QoutaAllocation { get; set; }
        public PersonalType PersonalType { get; set; }
        public Foods MainFood { get; set; }
        public Foods Dessert { get; set; }
        public Foods SideDish { get; set; }
        public OrganGarrison OrganGarrison { get; set; }
        public Person Person { get; set; }
        public FoodReceiverType FoodReceiverType { get; set; }
        public FoodTokenType FoodTokenType { get; set; }

        #endregion
    }
}