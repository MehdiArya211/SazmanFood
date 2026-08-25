using Domain.Entities;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using Infrastructure.Data;

namespace BLL.Interface
{
    public interface IFoodSourceManager
        : IManager<FoodSource, ApplicationContext>
    {
        DataTableResponseDTO<FoodSourceDataTableDTO>
            GetDataTableDTO(
                DataTableSearchDTO searchData,
                FoodSourceFilterDataTableDTO filters);

        IList<SelectListDTO> GetSelectListPersonalTypeDTO();

        IList<SelectListDTO> GetSelectListYeganTypeDTO();

        BaseResult Create(CreatFoodSourceDTO model);

        EditFoodSourceDTO GetEditDTO(long id);

        BaseResult Update(EditFoodSourceDTO model);
    }
}