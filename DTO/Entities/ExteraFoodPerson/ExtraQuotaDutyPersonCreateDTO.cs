using System.ComponentModel.DataAnnotations;

namespace DTO.Entities
{
    public class ExtraQuotaDutyPersonCreateDTO
    {
        [Required]
        public long RequestId { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} باید 10 رقم باشد.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "{0} باید شامل 10 رقم باشد.")]
        public string NationalCode { get; set; }

        [Display(Name = "شماره پرسنلی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string PersonCode { get; set; }

        [Display(Name = "نام و نشان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string FullName { get; set; }
    }
}