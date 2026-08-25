using DAL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class YearRepository : Repository<Years>, IYearRepository
{
    public YearRepository(DbContext _Context) : base(_Context)
    {
    }


}
