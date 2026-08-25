using DAL.Interface;
using Domain.Entities.FoodManage;
using DTO.DataTable;
using DTO.Entities.FoodMang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IFoodRepository : IRepository<Foods>
    {

        DataTableResponseDTO<FoodDTO> GetDataTableDTO(DataTableSearchDTO searchData, FoodFilterDTO filters);

    }

}
