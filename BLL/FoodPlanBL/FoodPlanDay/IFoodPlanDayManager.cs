using Domain.Entities;
using DTO;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IFoodPlanDayManager : IManager<FoodPlanDay, ApplicationContext>
    {

        DataTableResponseDTO<FoodPlanDayDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData);
        DataTableResponseDTO<FoodPlanDayDataTableDTO> GetFoodPlanDayDataTableDTO(DataTableSearchDTO searchData, long FoodPlanId);

        IList<SelectListDTO> GetSelectListDTO();
        IList<SelectListDTO> GetSelectListMealDTO();
        IList<SelectListDTO> GetSelectListDaysDTO();
        IList<SelectListDTO> GetSelectListFoodDTO();
        IList<SelectListDTO> GetSelectListFoodDesserDTO();
        IList<SelectListDTO> GetSelectListFoodDorchinDTO();
        BaseResult AddFoodPlanDay(FoodPlanDayDataTableDTO model);
        FoodPlanDayDataTableDTO GetFoodPlanDayEditDTO(long id);
        BaseResult UpdateFoodPlanDay(FoodPlanDayDataTableDTO model);
        BaseResult GetAllDay();
        /// <summary>
        /// نمایش تمام غذاهای روزانه هر یگان براساس سهمیه بندی و روز و وعده ی غذایی
        /// </summary>
        /// <param name="OrganGarrisonId"></param>
        /// <param name="dayId"></param>
        /// <param name="mealId"></param>
        /// <returns></returns>
        List<FoodPlanDayListForSahmiyebandiDTO> GetAllFoodForSahmiyebandikadrandvazifeh(
               long dayId, long mealId, long yearsId);


    }
}
