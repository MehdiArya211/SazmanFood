using Domain.Entities;
using System.Collections.Generic;

namespace DAL.Interface
{
    public interface IRoleMenuRepository : IRepository<RoleMenu>
    {

        /// <summary>
        /// گرفتن همه دسترسی های یک نقش خاص
        /// </summary>
        /// <param name="RoleId">شناسه نقش</param>
        /// <returns></returns>
        IEnumerable<RoleMenu> GetByRoleId(long? RoleId);
    }
}
