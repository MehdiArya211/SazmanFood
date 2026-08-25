using BLL.Interface;
using Domain.Entities.FoodManage;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using DTO.Entities.DiningHalDTo;
using Infrastructure.Data;

namespace BLL.FoodManag.DiningHallBL
{
    public interface IDiningHallManager
        : IManager<DiningHall, ApplicationContext>
    {
        DataTableResponseDTO<DiningHallDTO>
            GetDataTableDTO(
                DataTableSearchDTO searchData,
                DiningHallFilterDTO filters);

        BaseResult CreateDiningHalls(
            DiningHallCreateDTO model);

        DiningHallEditDTO GetDiningHallForEditDTO(
            long? id);

        BaseResult UpdateDiningHall(
            DiningHallEditDTO model);

        int GetTotalCount();

        bool TitleIsUnique(
            string title,
            int orgId,
            long? id = null);

        bool InternalDiningHallExists(
            int orgId,
            long? id = null);
    }
}