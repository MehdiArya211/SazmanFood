using System.ComponentModel.DataAnnotations;

namespace DTO.Entities
{
    public class ExtraQuotaOfficialPersonCreateDTO
    {
        [Required]
        public long RequestId { get; set; }

        public long? PersonId { get; set; }

        [Display(Name = "کد پرسنلی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string PersonCode { get; set; }

        public string NationalCode { get; set; }

        public string RankTitle { get; set; }

        [Display(Name = "نام و نشان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string FullName { get; set; }

        [Display(Name = "سالن غذاخوری")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long? DiningHallId { get; set; }
    }
}