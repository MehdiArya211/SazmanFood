using Domain.Entities;
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
    public interface IFoodPlanManager : IManager<FoodPlan, ApplicationContext>
    {
        DataTableResponseDTO<FoodPlanDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, FoodPlanFilterDataTableDTO filters);

        BaseResult Create(CreatFoodPlanDTO model);
        EditFoodPlanDTO GetEditDTO(long id);
        BaseResult Update(EditFoodPlanDTO model);
        string GetDayTitle(long dayId);
        IList<SelectListDTO> GetSelectListSeasonDTO();
        IList<SelectListDTO> GetSelectListYearsDTO();
    }
}
