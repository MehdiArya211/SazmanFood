using DTO.Entities.FoodReservation;

namespace DTO;

public class FoodPlanDTO
{
    public List<FoodReserveDetailDTO1> Details { get; set; } = new List<FoodReserveDetailDTO1>();
    public bool IsEditing { get; set; } = false;
}

public class FoodReserveDetailDTO1
{
    public long Id { get; set; }
    public long DayId { get; set; }
    public long MealId { get; set; }
    public long? MainFoodId { get; set; }
    public long? DessertId { get; set; }
    public long? SideDishId { get; set; }

    // برای نمایش در View
    public string DayTitle { get; set; }
    public string MealTitle { get; set; }
    public string MainFoodTitle { get; set; }
    public string DessertTitle { get; set; }
    public string SideDishTitle { get; set; }
}

public class FoodReserveDTO1
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<FoodReserveDetailDTO1> Details { get; set; }
}
