using DAL.Interface;
using Domain.Entities;
using DTO.DataTable;
using DTO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL;

public interface IUnitStatisticRepository
    : IRepository<UnitStatistic>
{
    DataTableResponseDTO<UnitStatisticDTO>
        GetDataTableDTO(
            DataTableSearchDTO searchData,
            UnitStatisticFilterDTO filters);
}
