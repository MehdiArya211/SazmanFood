using BLL.Interface;
using Domain.Entities.FoodReservation;
using DTO.Base;
using DTO.Entities.FoodReservation;
using Infrastructure.Data;

namespace BLL.ReserveManagment
{
    public interface IFoodReserveDetailManager : IManager<FoodReserveDetail, ApplicationContext>
    {

        public BaseResult CreateFoodReserveDetail(List<FoodReserveDetailDTO> foodReserveDetailModel , long foodReserveId);
    }
}
