using Domain.Entities.FoodManage;
using DTO.DataTable;
using DTO.Entities;
using DTO.Entities.DiningHalDTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IDiningHallRepository : IRepository<DiningHall>
    {
        DataTableResponseDTO<DiningHallDTO> GetDataTableDTO(DataTableSearchDTO searchData, DiningHallFilterDTO filters);

    }
}
