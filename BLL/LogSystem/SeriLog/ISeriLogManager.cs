using BLL.Interface;
using Domain.Entities.LogSystem;
using DTO;
using DTO.DataTable;
using DTO.Entities.LogSystem;
using Infrastructure.Data;

namespace BLL
{
    public interface ISeriLogManager : IManager<SeriLog, ApplicationContext>
    {
        DataTableResponseDTO<SeriLogDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData , SeriLogFilter filters);

    }
}
