using BLL.Interface;
using Domain.Entities;
using DTO.DataTable;
using DTO.Entities.LogSystem.UserChangeLogEvent;
using DTO.UserLog;
using Infrastructure.Data;

namespace BLL.LogSystem.UserChangeLogEvent;

public interface IUserChangeLogEventManager : IManager<DbChangeLogEvent, LogContext>
{
    /// <summary>
    /// گرفتن لیست لاگ ها برای نمایش در پنل مدیریت
    /// </summary>
    /// <returns></returns>
    DataTableResponseDTO<UserChagneLogEventDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserChagneLogEventFilter filters);

    UserChagneLogEventDTO GetDetailDTO(long id);
}
