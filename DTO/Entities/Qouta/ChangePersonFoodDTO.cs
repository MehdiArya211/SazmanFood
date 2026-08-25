using System.ComponentModel.DataAnnotations;

namespace DTO.Entities;

/// <summary>
/// مدل تغییر غذای پرسنل
/// </summary>
public class ChangePersonFoodDTO
{
    [Display(Name = "شناسه تخصیص غذا")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long QoutaPersonId { get; set; }

    [Display(Name = "غذای اصلی")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long MainFoodId { get; set; }

    [Display(Name = "دسر")]
    public long? DessertId { get; set; }

    [Display(Name = "دورچین")]
    public long? SideDishId { get; set; }
}