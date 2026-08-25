using Domain.Entities.FoodReservation;
using DTO.Base;
using DTO.Entities.FoodReservation;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace BLL.ReserveManagment
{
    public class FoodReserveDetailManager : Manager<FoodReserveDetail, ApplicationContext>, IFoodReserveDetailManager
    {
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly ISession Session;
        public FoodReserveDetailManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor) : base(_Context, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }

        public BaseResult CreateFoodReserveDetail(List<FoodReserveDetailDTO> foodReserveDetailModel, long foodReserveId)
        {

            foreach (var item in foodReserveDetailModel)
            {
                var foodReserveDetail = new FoodReserveDetail();
                foodReserveDetail.FoodReserveId = foodReserveId;
                foodReserveDetail.DayId = item.DayId;
                foodReserveDetail.MealId = item.MealId;
                foodReserveDetail.MainFoodId = item.MainFoodId;
                foodReserveDetail.DessertId = item.DessertId;
                foodReserveDetail.SideDishId = item.SideDishId;

                var res = base.Create(foodReserveDetail);
            }
           var result= UOW.Commit();

            if (result)
            {
                return new BaseResult()
                {
                    Message = "جزئیات غذای رزرو شده با موفقیت ثبت شد",
                    Status = result
                };
            }

            return new BaseResult()
            {
                Message = "خطا در ثبت جزئیات غذای رزرو شده",
                Status = result
            };

        }
    }
}
