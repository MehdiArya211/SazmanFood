using Domain.Entities;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Utilities.Extentions;

namespace DTO.Entities
{
    public class FoodSourceDataTableDTO
    {
        public long Id { get; set; }

        [Display(Name = "نوع خدمت")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long YeganTypeId { get; set; }

        [Display(Name = "نوع خدمت")]
        public string YeganTypeName { get; set; }

        [Display(Name = "نوع پرسنل")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long PersonalTypeId { get; set; }

        [Display(Name = "نوع پرسنل")]
        public string PersonalTypeName { get; set; }

        [Display(Name = "نوع روز")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public DayType DayType { get; set; }

        [Display(Name = "نوع روز")]
        public string DayTypeTitle { get; set; }

        [Display(Name = "درصد صبحانه")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [Range(0, 100, ErrorMessage = "{0} باید بین صفر تا صد باشد.")]
        public double PercentBreakfast { get; set; }

        [Display(Name = "درصد ناهار")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [Range(0, 100, ErrorMessage = "{0} باید بین صفر تا صد باشد.")]
        public double PercentLunch { get; set; }

        [Display(Name = "درصد شام")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [Range(0, 100, ErrorMessage = "{0} باید بین صفر تا صد باشد.")]
        public double PercentDinner { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public string CreateDateFa
        {
            get
            {
                return CreateDate.ToPersianDateTime().ToString();
            }
        }

        public long UserCreateId { get; set; }
        public bool? IsDeleted { get; set; }

        public static Expression<Func<FoodSource, FoodSourceDataTableDTO>>
            Selector
        {
            get
            {
                return model => new FoodSourceDataTableDTO
                {
                    Id = model.Id,
                    YeganTypeId = model.YeganTypeId,
                    YeganTypeName = model.YeganType.Title,
                    PersonalTypeId = model.PersonalTypeId,
                    PersonalTypeName = model.PersonalType.Title,
                    DayType = model.DayType,
                    DayTypeTitle =
                        model.DayType == DayType.Normal
                            ? "عادی"
                            : model.DayType == DayType.Holiday
                                ? "تعطیل"
                                : "نیمه‌تعطیل",
                    PercentBreakfast = model.PercentBreakfast,
                    PercentLunch = model.PercentLunch,
                    PercentDinner = model.PercentDinner,
                    IsDeleted = model.IsDeleted,
                    CreateDate = model.CreateDate
                };
            }
        }
    }

    public class CreatFoodSourceDTO : FoodSourceDataTableDTO
    {
    }

    public class EditFoodSourceDTO : FoodSourceDataTableDTO
    {
        public static Expression<Func<FoodSource, EditFoodSourceDTO>>
            Selector
        {
            get
            {
                return model => new EditFoodSourceDTO
                {
                    Id = model.Id,
                    YeganTypeId = model.YeganTypeId,
                    YeganTypeName = model.YeganType.Title,
                    PersonalTypeId = model.PersonalTypeId,
                    PersonalTypeName = model.PersonalType.Title,
                    DayType = model.DayType,
                    DayTypeTitle =
                        model.DayType == DayType.Normal
                            ? "عادی"
                            : model.DayType == DayType.Holiday
                                ? "تعطیل"
                                : "نیمه‌تعطیل",
                    PercentBreakfast = model.PercentBreakfast,
                    PercentLunch = model.PercentLunch,
                    PercentDinner = model.PercentDinner,
                    IsDeleted = model.IsDeleted,
                    CreateDate = model.CreateDate
                };
            }
        }
    }

    public class FoodSourceFilterDataTableDTO
    {
        public long? YeganTypeId { get; set; }
        public long? PersonalTypeId { get; set; }
        public DayType? DayType { get; set; }
    }
}