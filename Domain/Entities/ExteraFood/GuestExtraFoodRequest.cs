using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class GuestExtraFoodRequest : EntityBase
    {
        [Display(Name = "یگان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int OrgId { get; set; }

        [Display(Name = "نام یگان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(200)]
        public string OrgTitle { get; set; }

        [Display(Name = "وعده")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long MealId { get; set; }

        [Display(Name = "نوع پرسنل")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long PersonalTypeId { get; set; }

        [Display(Name = "نوع خدمت")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long YeganTypeId { get; set; }

        [Display(Name = "از تاریخ")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public DateTime FromDate { get; set; }

        [Display(Name = "تا تاریخ")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public DateTime ToDate { get; set; }

        [Display(Name = "تعداد مهمان")]
        [Range(0, int.MaxValue)]
        public int GuestCount { get; set; }

        [Display(Name = "تعداد مازاد بر سهمیه")]
        [Range(0, int.MaxValue)]
        public int ExtraQuotaCount { get; set; }

        [Display(Name = "وضعیت")]
        public GuestExtraFoodRequestStatus Status { get; set; }

        public long CreatorId { get; set; }

        [MaxLength(200)]
        public string CreatorFullName { get; set; }

        public DateTime RequestCreateDate { get; set; }

        public long? SenderId { get; set; }

        [MaxLength(200)]
        public string SenderFullName { get; set; }

        public DateTime? SendDate { get; set; }

        public long? ApproverId { get; set; }

        [MaxLength(200)]
        public string ApproverFullName { get; set; }

        public DateTime? ApproveDate { get; set; }

        public Meal Meal { get; set; }

        public PersonalType PersonalType { get; set; }

        public YeganType YeganType { get; set; }

        public List<GuestExtraFoodRequestAttachment> Attachments
        {
            get;
            set;
        } = new();
    }
}