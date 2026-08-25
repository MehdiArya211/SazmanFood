using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// برنامه غذایی
/// </summary>
public class FoodPlan : EntityBase
{


    [Display(Name = "سال")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long YearsId { get; set; }

    [Display(Name = "فصل")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long SeasonId { get; set; }

    [Display(Name = "یگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public int OrganId { get; set; }

    [Display(Name = "یگان")]
    [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public string OrganTitle { get; set; }

    [Display(Name = "وضعیت  ")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public bool IsActive { get; set; }

   
    #region relation
    public Years Years { get; set; }
    public Season Season { get; set; }

    public List<FoodPlanDay> FoodPlanDay { get; set; }

    #endregion

}
