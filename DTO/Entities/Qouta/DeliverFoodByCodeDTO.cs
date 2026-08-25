using System.ComponentModel.DataAnnotations;

namespace DTO.Entities.QoutaPerson
{
    /// <summary>
    /// مدل تحویل غذا با کد ژتون
    /// </summary>
    public class DeliverFoodByCodeDTO
    {
        [Display(Name = "کد تحویل")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string DeliveryCode { get; set; }

        [Display(Name = "شناسه وعده غذایی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long MealId { get; set; }
    }
}