using DAL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        public MenuRepository(DbContext _Context) : base(_Context)
        {
        }





    }
}
