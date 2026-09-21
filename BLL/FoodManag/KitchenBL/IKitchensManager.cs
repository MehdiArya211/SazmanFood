using BLL.Interface;
using Domain.Entities.FoodManage;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities.FoodMang;
using DTO.Entities.Kitchen;
using Infrastructure.Data;
using System;

namespace BLL.FoodManag.KitchenBL
{
    public interface IKitchensManager : IManager<Kitchens, ApplicationContext>
    {
        DataTableResponseDTO<KitchenDTO> GetDataTableDTO(DataTableSearchDTO searchData, KitchenFilterDTO filters);
        BaseResult CreateKitchens(KitchenCreateDTO creatkitchen);

        KitchenEditDTO GetKitchenForEditDTO(long? id);

        KitchenCookingStatisticsDTO GetCookingStatistics(
            DateTime fromDate,
            DateTime toDate,
            long? kitchenId,
            long? mealId);
        BaseResult UpdateKitchens(KitchenEditDTO kitchenedit);
    }
}
