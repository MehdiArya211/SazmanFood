using Domain.Entities.FoodReservation;
using Microsoft.EntityFrameworkCore;

namespace DAL.ReserveManagment
{
    public class FoodReserveRepository : Repository<FoodReserve>, IFoodReserveRepository
    {
        public FoodReserveRepository(DbContext _Context) : base(_Context)
        {
        }
    }
}
