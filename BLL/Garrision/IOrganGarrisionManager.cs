using BLL.Interface;
using Domain.Entities.Garrison;
using DTO;
using DTO.Base;
using DTO.DataTable;
using Infrastructure.Data;

namespace BLL
{
    public interface IOrganGarrisionManager : IManager<OrganGarrison, ApplicationContext>
    {
        DataTableResponseDTO<OrganGarrisonDTO> GetDataTableDTO(DataTableSearchDTO searchData, OrganGarrisonDTO filters);

        List<OrganGarrisonType> GetorganGarrisonTypes();

        IList<SelectListDTO> GetSelectListDTO();

        IList<SelectListDTO> GetSubSelectListDTO(long parentId);

        BaseResult Create(OrganGarrisonDTO model, long userId);

        List<OrganGarrison> GetParent();

        OrganGarrisonDTO LoadEditForm(long id);

        BaseResult Update(OrganGarrisonDTO model);
    }
}