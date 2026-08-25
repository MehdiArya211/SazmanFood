using DAL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DAL
{
    public class RoleMenuRepository : Repository<RoleMenu>, IRoleMenuRepository
    {
        public RoleMenuRepository(DbContext _Context) : base(_Context)
        {
        }



        /// <summary>
        /// گرفتن همه دسترسی های یک نقش خاص
        /// </summary>
        /// <param name="RoleId">شناسه نقش</param>
        /// <returns></returns>
        public IEnumerable<RoleMenu> GetByRoleId(long? RoleId)
        {
            if (RoleId == null) return null;
            return Get(x => x.RoleId == RoleId);
        }


    }
}
