using Domain.Entities.FoodManage;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// نفرات ثبت‌شده در سهمیه یگان
/// </summary>
public class UnitQuotaPerson : EntityBase
{
    [Display(Name = "آمار یگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long UnitStatisticId { get; set; }

    [Display(Name = "سهمیه یگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long UnitQuotaId { get; set; }

    [Display(Name = "یگان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public int OrgId { get; set; }

    [Display(Name = "نوع روز")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public DayType DayType { get; set; }

    [Display(Name = "وعده")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long MealId { get; set; }

    [Display(Name = "نوع پرسنل")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long PersonalTypeId { get; set; }

    [Display(Name = "سالن غذاخوری")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long DiningHallId { get; set; }

    [Display(Name = "شناسه پرسنل")]
    public long? PersonId { get; set; }

    [Display(Name = "کد پرسنلی")]
    [MaxLength(20)]
    public string PersonCode { get; set; }

    [Display(Name = "کد ملی")]
    [MaxLength(10)]
    public string NationalCode { get; set; }

    [Display(Name = "درجه")]
    [MaxLength(100)]
    public string RankTitle { get; set; }

    [Display(Name = "نام و نشان")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [MaxLength(200)]
    public string FullName { get; set; }

    [Display(Name = "ثبت‌کننده")]
    public long CreatorId { get; set; }

    [Display(Name = "ثبت‌کننده")]
    [MaxLength(200)]
    public string CreatorFullName { get; set; }

    [Display(Name = "تاریخ ثبت")]
    public DateTime CreateDate { get; set; }

    #region Relation

    public UnitStatistic UnitStatistic { get; set; }
    public UnitQuota UnitQuota { get; set; }
    public Meal Meal { get; set; }
    public PersonalType PersonalType { get; set; }
    public DiningHall DiningHall { get; set; }

  //  public List<UnitQuotaPerson> Persons { get; set; } = new();


    #endregion
}