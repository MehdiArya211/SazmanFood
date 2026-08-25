using DAL.Interface;
using Domain.Entities.FoodManage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class FoodTypesRepository : Repository<FoodTypes>, IFoodTypesRepository
    {
        public FoodTypesRepository(DbContext _Context) : base(_Context)
        {

        }
    }
}
