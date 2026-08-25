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
    public interface IFoodSourceRepository : IRepository<FoodSource>
    {

        DataTableResponseDTO<FoodSourceDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, FoodSourceFilterDataTableDTO filters);
    }
}
