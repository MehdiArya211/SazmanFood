using DAL.Interface;
using Domain.Entities.Garrison;
using DTO;
using DTO.DataTable;

namespace DAL
{
    public interface IOrganGarrisonRepository : IRepository<OrganGarrison>
	{
        DataTableResponseDTO<OrganGarrisonDTO> GetDataTableDTO(DataTableSearchDTO searchData, OrganGarrisonDTO filters);

    }
}
