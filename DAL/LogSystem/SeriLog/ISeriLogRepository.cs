using DAL.Interface;
using Domain.Entities.LogSystem;
using DTO;
using DTO.DataTable;
using DTO.Entities.LogSystem;
using DTO.Entities.LogSystem.UserChangeLogEvent;

namespace DAL
{
    public interface ISeriLogRepository : IRepository<Domain.Entities.LogSystem.SeriLog>
    {
        /// <summary>
        /// گرفتن لیست لاگ ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        DataTableResponseDTO<SeriLogDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, SeriLogFilter filters);
    }
}
