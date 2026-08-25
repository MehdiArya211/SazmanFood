using DAL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class YearsRepository : Repository<Years>, IYearsRepository
    {
        public YearsRepository(DbContext _Context) : base(_Context)
        {
        }
    }
}
