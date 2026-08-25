using Domain.Entities.FoodReservation;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entities.FoodReservation
{
    public class EditReserveDTO
    {
        public List<Days> Days { get; set; }
        public List<Meal> Meals { get; set; }

        public Dictionary<(long dayId, long mealId), dynamic> FoodsByDayAndMeal { get; set; }
        public Dictionary<(long dayId, long mealId), FoodReserve> UserReserves { get; set; }
    }

}
