using Domain.Entities;
using Domain.Entities.LogSystem;
using DTO.DataTable;
using DTO.Entities.LogSystem.UserChangeLogEvent;
using DTO.UserLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IUserChangeLogEventRepository : IRepository<TrackDatabaseLog>
    {
        /// <summary>
        /// گرفتن لیست لاگ ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        DataTableResponseDTO<UserChagneLogEventDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserChagneLogEventFilter filters);
        UserChagneLogEventDTO GetDetailDTO(long id);
    }
}
