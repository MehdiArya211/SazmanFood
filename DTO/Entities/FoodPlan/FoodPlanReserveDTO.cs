using Domain.Entities;
using Domain.Entities.FoodManage;
using Domain.Entities.FoodReservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entities
{
    public class FoodPlanReserveDTO
    {
        public List<Days> Days { get; set; }
        public List<Meal> Meals { get; set; }

        public Dictionary<(long DayId, long MealId), List<Foods>> MainFoods { get; set; } = new();
        public Dictionary<(long DayId, long MealId), List<Foods>> Desserts { get; set; } = new();
        public Dictionary<(long DayId, long MealId), List<Foods>> SideDishes { get; set; } = new();

        // **جدید**: برای نگهداری رزرو قبلی کاربر در هفته جاری
        public FoodReserve ExistingFoodReserve { get; set; }
        public List<FoodReserveDetail> ExistingReserveDetails { get; set; }
    }


}
