using System.ComponentModel.DataAnnotations;

namespace DTO.Entities;

/// <summary>
/// مدل ثبت سهمیه غذا توسط آماد
/// </summary>
public class QoutaAllocationCreateDTO
{
    [Display(Name = "پادگان")]
    public long? OrganGarrisonParentId { get; set; }

    [Display(Name = "یگان پادگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long OrganGarrisonId { get; set; }

    [Display(Name = "برنامه غذایی روز")]
    public long? FoodPlanDayId { get; set; }

    [Display(Name = "تاریخ سهمیه بندی")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public DateTime QoutaAllocationDate { get; set; }

    [Display(Name = "روز")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long DayId { get; set; }

    [Display(Name = "وعده غذایی")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long MealId { get; set; }

    [Display(Name = "ظرفیت کادر")]
    public int OfficerCapacity { get; set; }

    [Display(Name = "ظرفیت وظیفه")]
    public int SoldierCapacity { get; set; }

    [Display(Name = "ظرفیت مهمان")]
    public int GuestCapacity { get; set; }

    [Display(Name = "ظرفیت ژتون مدیریتی")]
    public int ManagementTokenCapacity { get; set; }

    [Display(Name = "مهلت ثبت غذا")]
    public DateTime? RegisterDeadline { get; set; }

    [Display(Name = "اجازه تغییر توسط کاربر")]
    public bool AllowPersonChange { get; set; }

    [Display(Name = "اجازه ثبت توسط اداری")]
    public bool AllowOfficeUserRegister { get; set; }

    [Display(Name = "وضعیت نهایی")]
    public bool IsFinalized { get; set; }

    public long UserCreateId { get; set; }
}