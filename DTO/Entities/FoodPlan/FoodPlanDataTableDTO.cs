using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utilities.Extentions;

namespace DTO.Entities
{
    public class FoodPlanDataTableDTO
    {
        public long Id { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long YearsId { get; set; }

        [Display(Name = "سال")]
        public string YearsTitle { get; set; }

        [Display(Name = "فصل")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long SeasonId { get; set; }

        [Display(Name = "فصل")]
        public string SeasonTitle { get; set; }

        [Display(Name = "یگان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int OrganId { get; set; }

        [Display(Name = "یگان")]
        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string OrganTitle { get; set; }

        [Display(Name = "وضعیت  ")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public bool IsActive { get; set; }


        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "تاریخ ایجاد")]
        public string CreateDateFa { get { return CreateDate.ToPersianDateTime().ToString(); } }

        [Display(Name = "شناسه ایجادکننده")]
        public long UserCreateId { get; set; }

        [Display(Name = "حذف شده")]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// فیلدهای لازم برای استخراج مدل از موجودیت مربوطه
        /// </summary>
        public static Expression<Func<FoodPlan, FoodPlanDataTableDTO>> Selector
        {
            get
            {
                return model => new FoodPlanDataTableDTO()
                {
                    Id = model.Id,
                    OrganId = model.OrganId,
                    OrganTitle = model.OrganTitle,
                    YearsTitle = model.Years.Title,
                    YearsId = model.YearsId,
                    SeasonId = model.SeasonId,
                    SeasonTitle = model.Season.Title,
                   
                    IsDeleted = model.IsDeleted,

                    CreateDate = model.CreateDate,
                };
            }
        }

    }

    #region CREATE
    public class CreatFoodPlanDTO : FoodPlanDataTableDTO
    {
        public static Expression<Func<FoodPlan, CreatFoodPlanDTO>> Selector
        {
            get
            {
                return model => new CreatFoodPlanDTO()
                {
                    Id = model.Id,
                    OrganId = model.OrganId,
                    OrganTitle = model.OrganTitle,
                    YearsTitle = model.Years.Title,
                    YearsId = model.YearsId,
                    SeasonId = model.SeasonId,
                    SeasonTitle = model.Season.Title,
                    IsDeleted = model.IsDeleted,

                    CreateDate = model.CreateDate,
                };
            }
        }

    }
    #endregion

    #region EDIT
    public class EditFoodPlanDTO : FoodPlanDataTableDTO
    {
        public static Expression<Func<FoodPlan, EditFoodPlanDTO>> Selector
        {
            get
            {
                return model => new EditFoodPlanDTO()
                {
                    Id = model.Id,
                    OrganId = model.OrganId,
                    OrganTitle = model.OrganTitle,
                    YearsTitle = model.Years.Title,
                    YearsId = model.YearsId,
                    SeasonId = model.SeasonId,
                    SeasonTitle = model.Season.Title,

                    IsDeleted = model.IsDeleted,
                    IsActive = model.IsActive,

                    CreateDate = model.CreateDate,
                };
            }
        }
    }
    #endregion

    #region Filter
    public class FoodPlanFilterDataTableDTO
    {
        [Display(Name = "شناسه یگان")]
        public int OrgId { get; set; }

     
        [Display(Name = "تاریخ شروع")]
        public DateTime? StartDate { get; set; }


        [Display(Name = "تاریخ پایان")]
        public DateTime? EndDate { get; set; }

    }
    #endregion

    #region FoodPlanDayDataTableDTO
    public class FoodPlanDayDataTableDTO
    {
        [Display(Name = "شناسه")]
        public long Id { get; set; }

        [Display(Name = "شناسه ")]
        public long FoodPlanDayId { get; set; }

        [Display(Name = "شناسه برنامه غذایی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long FoodPlanId { get; set; }

        [Display(Name = "روز")]
        public string DayName { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long DayId { get; set; }

        [Display(Name = "وعده غذایی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long MealId { get; set; }


        [Display(Name = "وعده غذایی")]
        public string MealName { get; set; }

        [Display(Name = "غذا")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long FoodId { get; set; }
        [Display(Name = "غذا")]
        public string FoodTitle { get; set; }

        [Display(Name = "دسر")]
        public long? FoodDesserId { get; set; }

        [Display(Name = "دسر")]
        public string FoodDesserTitle { get; set; }

        [Display(Name = "دورچین")]
        public long? FoodDorchinId { get; set; }


        [Display(Name = "دورچین")]
        public string FoodDorchinTitle { get; set; }

        [Display(Name = "تعداد")]
        public int? Count { get; set; }


        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "تاریخ ایجاد")]
        public string CreateDateFa { get { return CreateDate.ToPersianDateTime().ToString(); } }

        [Display(Name = "شناسه ایجادکننده")]
        public long UserCreateId { get; set; }

        [Display(Name = "حذف شده")]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// فیلدهای لازم برای استخراج مدل از موجودیت مربوطه
        /// </summary>
        public static Expression<Func<FoodPlanDay, FoodPlanDayDataTableDTO>> Selector
        {
            get
            {
                return model => new FoodPlanDayDataTableDTO()
                {
                    Id = model.Id,
                    FoodPlanDayId = model.Id,
                    FoodPlanId = model.FoodPlanId,
                    FoodId = model.FoodId,
                    FoodDesserId = model.FoodDesserId,
                    FoodDorchinId = model.FoodDorchinId,
                    FoodTitle = model.Food.Title,
                    FoodDesserTitle = "",
                    MealId = model.MealId,
                    MealName = model.Meal.Title,
                    DayId = model.DayId,
                    DayName = model.Day.Title,
                    Count = model.Count,
                    IsDeleted = model.IsDeleted,

                    CreateDate = model.CreateDate,
                };
            }
        }

    }
    #endregion


}
