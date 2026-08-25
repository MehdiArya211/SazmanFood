using Domain.Entities.FoodManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.FoodReservation
{
    public class FoodReserveDetail
    {
        public long Id { get; set; }
        public long FoodReserveId { get; set; }
        public long DayId { get; set; }
        public long MealId { get; set; }
        public long? MainFoodId { get; set; }
        public long? DessertId { get; set; }
        public long? SideDishId { get; set; }

        public FoodReserve FoodReserve { get; set; }
        public Days Day { get; set; }
        public Meal Meal { get; set; }
        public Foods MainFood { get; set; }
        public Foods Dessert { get; set; }
        public Foods SideDish { get; set; }
    }

}
