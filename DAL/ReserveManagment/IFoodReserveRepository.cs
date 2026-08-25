using DAL.Interface;
using Domain.Entities;
using Domain.Entities.FoodReservation;

namespace DAL.ReserveManagment
{
    public interface IFoodReserveRepository : IRepository<FoodReserve>
    {
    }
}
