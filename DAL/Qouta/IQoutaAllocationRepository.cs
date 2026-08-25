using DAL.Interface;
using Domain.Entities;
using DTO;
using DTO.DataTable;
using DTO.Entities;

namespace DAL
{
	public interface IQoutaAllocationRepository : IRepository<QoutaAllocation>
	{
        DataTableResponseDTO<QoutaAllocationDataTableDTO> GetDataTableDTO(
            DataTableSearchDTO searchData,
            QoutaAllocationUserAccessDTO userAccess,
            QoutaAllocationFilterDataTableDTO filters);
    }
}
