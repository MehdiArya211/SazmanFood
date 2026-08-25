namespace DTO.Entities.FoodReservation;

public class WeeklyFoodReserveDTO
{
    public long FoodReserveDetaileId { get; set; }
    public DateTime Date { get; set; }
    public string DayTitle { get; set; }
    public string MealTitle { get; set; }
    public string MainFood { get; set; }
    public string Dessert { get; set; }
    public string SideDish { get; set; }
}
