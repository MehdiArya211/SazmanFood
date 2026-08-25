using Domain.Entities.FoodReservation;
using Microsoft.EntityFrameworkCore;

namespace DAL.ReserveManagment
{
    public class FoodReservationDetailRepository : Repository<FoodReserveDetail>, IFoodReservationDetailRepository
    {
        public FoodReservationDetailRepository(DbContext _Context) : base(_Context)
        {
        }
    }
}
