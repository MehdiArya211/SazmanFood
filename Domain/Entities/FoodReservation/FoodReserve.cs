namespace Domain.Entities.FoodReservation
{
    public class FoodReserve
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<FoodReserveDetail> Details { get; set; }
    }


}
