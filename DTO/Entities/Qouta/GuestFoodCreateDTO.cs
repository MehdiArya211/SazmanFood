using System.ComponentModel.DataAnnotations;

namespace DTO.Entities;

/// <summary>
/// مدل ثبت غذا برای مهمان
/// </summary>
public class GuestFoodCreateDTO
{
    [Display(Name = "شناسه سهمیه")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long QoutaAllocationId { get; set; }

    [Display(Name = "نام مهمان")]
    [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
    public string FName { get; set; }

    [Display(Name = "نام خانوادگی مهمان")]
    [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
    public string LName { get; set; }

    [Display(Name = "تعداد مهمان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public int Count { get; set; }

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

    [Display(Name = "توضیحات")]
    [MaxLength(500, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
    public string Description { get; set; }
}