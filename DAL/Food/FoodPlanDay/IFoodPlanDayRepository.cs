using Domain.Entities;
using DTO.DataTable;
using DTO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IFoodPlanDayRepository : IRepository<FoodPlanDay>
    {
        DataTableResponseDTO<FoodPlanDayDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData);
        DataTableResponseDTO<FoodPlanDayDataTableDTO> GetFoodPlanDayDataTableDTO(DataTableSearchDTO searchData, long FoodPlanId);
    }
}
