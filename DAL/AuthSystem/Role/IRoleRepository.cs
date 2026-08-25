using Domain.Entities;
using DTO.DataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IRoleRepository : IRepository<Role>
    {
        /// <summary>
        /// گرفتن لیست نقشها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        DataTableResponseDTO<Role> GetDataTableDTO(DataTableSearchDTO searchData);

    }
}
