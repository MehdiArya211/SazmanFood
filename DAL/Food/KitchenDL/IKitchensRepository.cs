using DAL.Interface;
using Domain.Entities.FoodManage;
using DTO.DataTable;
using DTO.Entities.Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IKitchensRepository : IRepository<Kitchens>
    {
        DataTableResponseDTO<KitchenDTO> GetDataTableDTO(DataTableSearchDTO searchData, KitchenFilterDTO filters);

        KitchenCookingStatisticsDTO GetCookingStatistics(
            DateTime fromDate,
            DateTime toDate,
            long? kitchenId,
            long? mealId);

    }
}
