using BLL.Interface;
using Domain.Entities;
using DTO;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace BLL;

public class FoodPlanDayManager : Manager<FoodPlanDay, ApplicationContext>, IFoodPlanDayManager
{
    public FoodPlanDayManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
    {
    }

    /// <summary>
    /// گرفتن لیست کاربران برای نمایش در پنل مدیریت
    /// </summary>
    /// <returns></returns>
    public DataTableResponseDTO<FoodPlanDayDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData)
    {
        return UOW.FoodPlanDay.GetDataTableDTO(searchData);
    }

    public DataTableResponseDTO<FoodPlanDayDataTableDTO> GetFoodPlanDayDataTableDTO(DataTableSearchDTO searchData, long FoodPlanId)
    {
        return UOW.FoodPlanDay.GetFoodPlanDayDataTableDTO(searchData, FoodPlanId);
    }

    public IList<SelectListDTO> GetSelectListDTO()
    {
        return UOW.Days.GetDTO<SelectListDTO>(SelectListDTO.DaySelector).ToList();
    }
    public IList<SelectListDTO> GetSelectListMealDTO()
    {
        return UOW.Meal.GetDTO<SelectListDTO>(SelectListDTO.MealSelector).ToList();
    }
    public IList<SelectListDTO> GetSelectListDaysDTO()
    {
        return UOW.Days.GetDTO<SelectListDTO>(SelectListDTO.DaySelector).ToList();
    }
    public IList<SelectListDTO> GetSelectListFoodDTO()
    {
        return UOW.Food.GetDTO<SelectListDTO>(SelectListDTO.FoodsSelector, x => x.FoodTypesId.Value == 3).ToList();
    }
    public IList<SelectListDTO> GetSelectListFoodDesserDTO()
    {
        return UOW.Food.GetDTO<SelectListDTO>(SelectListDTO.FoodsSelector, x => x.FoodTypesId.Value == 2).ToList();
    }
    public IList<SelectListDTO> GetSelectListFoodDorchinDTO()
    {
        return UOW.Food.GetDTO<SelectListDTO>(SelectListDTO.FoodsSelector, x => x.FoodTypesId.Value == 1).ToList();
    }

    public BaseResult AddFoodPlanDay(FoodPlanDayDataTableDTO model)
    {
        var res = new BaseResult();

        var fPday = new FoodPlanDay()
        {
            FoodPlanId = model.FoodPlanId,
            DayId = model.DayId,
            MealId = model.MealId,
            FoodId = model.FoodId,
            FoodDesserId = model.FoodDesserId,
            FoodDorchinId = model.FoodDorchinId,
            Count = model.Count,
            RegUserId = model.UserCreateId,
            CreateDate = DateTime.Now,
            RegDate = DateTime.Now,
            IsDeleted = false,
        };
        return base.Create(fPday);
    }

    public FoodPlanDayDataTableDTO GetFoodPlanDayEditDTO(long id)
    {
        if (id == null) return null;
        return UOW.FoodPlanDay.GetOneDTO<FoodPlanDayDataTableDTO>(FoodPlanDayDataTableDTO.Selector, x => x.Id == id);
    }

    public BaseResult UpdateFoodPlanDay(FoodPlanDayDataTableDTO model)
    {
        var FPDEdit = UOW.FoodPlanDay.FirstOrDefault(x => x.Id == model.FoodPlanDayId);
        FPDEdit.FoodPlanId = model.FoodPlanId;
        FPDEdit.DayId = model.DayId;
        FPDEdit.MealId = model.MealId;
        FPDEdit.FoodId = model.FoodId;
        FPDEdit.FoodDesserId = model.FoodDesserId;
        FPDEdit.FoodDorchinId = model.FoodDorchinId;
        FPDEdit.Count = model.Count;
        FPDEdit.LastEditUserId = model.UserCreateId;
        FPDEdit.LastEditDate = DateTime.Now;

        return base.Update(FPDEdit);
    }

    public BaseResult GetAllDay()
    {
        var res = UOW.Days.GetAll().ToList();
        return new BaseResult()
        {

            Message = "",
            Model = res
        };
    }

    public List<FoodPlanDayListForSahmiyebandiDTO> GetAllFoodForSahmiyebandikadrandvazifeh0(
    int orgId, long dayId, long mealId, long yearsId)
    {
        var foods = UOW.FoodPlanDay
            .GetDTO(FoodPlanDayListForSahmiyebandiDTO.Selector,
            x => x.FoodPlan.OrganId == orgId && x.DayId == dayId && x.MealId == mealId && x.FoodPlan.YearsId==4)
            .ToList();

        return foods;
    }

    public List<FoodPlanDayListForSahmiyebandiDTO> GetAllFoodForSahmiyebandikadrandvazifeh(
         long dayId, long mealId, long yearsId)
    {
        var foods = UOW.FoodPlanDay
            .GetDTO(
                FoodPlanDayListForSahmiyebandiDTO.Selector,
                x =>
                    //x.FoodPlan.OrganId == orgId &&
                    x.FoodPlan.YearsId == yearsId &&
                    x.FoodPlan.IsActive &&
                    x.DayId == dayId &&
                    x.MealId == mealId
            )
            .ToList();

        // اگر برای dropdown می‌خوای تکراری‌ها حذف شود
        foods = foods
            .GroupBy(x => x.FoodId)
            .Select(g => g.First())
            .OrderBy(x => x.FoodTitle)
            .ToList();

        return foods;
    }





}
