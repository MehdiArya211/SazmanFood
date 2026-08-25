using System.ComponentModel.DataAnnotations;

namespace DTO.Entities.QoutaPerson
{
    /// <summary>
    /// مدل ثبت غذا برای پرسنل توسط کاربر اداری
    /// </summary>
    public class QoutaPersonOfficeCreateDTO
    {
        [Display(Name = "شناسه سهمیه")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long QoutaAllocationId { get; set; }

        [Display(Name = "شناسه پرسنل")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long PersonId { get; set; }

        [Display(Name = "نوع ژتون")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long FoodTokenTypeId { get; set; }

        [Display(Name = "غذای اصلی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long MainFoodId { get; set; }

        [Display(Name = "دسر")]
        public long? DessertId { get; set; }

        [Display(Name = "دورچین")]
        public long? SideDishId { get; set; }
    }
}