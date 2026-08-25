using Domain.Entities.FoodManage;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entities.FoodReservation
{
    public class FoodReserveDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<FoodReserveDetailDTO> Details { get; set; }
    }

    public class FoodReserveDetailDTO
    {
        public long Id { get; set; }
        public long FoodPlanId { get; set; }
       // public long FoodPlanId { get; set; }
        public long DayId { get; set; }
        public long MealId { get; set; }

        public long? MainFoodId { get; set; }
        public long? DessertId { get; set; }
        public long? SideDishId { get; set; }

        public FoodPlan FoodPlan { get; set; }
    }



}
