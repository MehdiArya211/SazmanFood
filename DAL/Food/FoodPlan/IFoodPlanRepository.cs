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
    public interface IFoodPlanRepository : IRepository<FoodPlan>
    {

        DataTableResponseDTO<FoodPlanDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, FoodPlanFilterDataTableDTO filters);
    }
}
