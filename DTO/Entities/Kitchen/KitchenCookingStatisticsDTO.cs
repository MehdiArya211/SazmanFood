namespace DTO.Entities.Kitchen;

public class KitchenCookingStatisticsDTO
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public long? KitchenId { get; set; }
    public long? MealId { get; set; }
    public List<KitchenCookingFilterDTO> Kitchens { get; set; } = new();
    public List<KitchenCookingFilterDTO> Meals { get; set; } = new();
    public List<KitchenCookingStatisticsRowDTO> Rows { get; set; } = new();

    public int TotalQuota => Rows.Sum(x => x.QuotaCount);
    public int TotalReserved => Rows.Sum(x => x.ReservedCount);
    public int TotalDelivered => Rows.Sum(x => x.DeliveredCount);
    public int TotalRemaining => Rows.Sum(x => x.RemainingCount);
}

public class KitchenCookingFilterDTO
{
    public long Id { get; set; }
    public string Title { get; set; }
}

public class KitchenCookingStatisticsRowDTO
{
    public long KitchenId { get; set; }
    public string KitchenTitle { get; set; }
    public string OrgTitle { get; set; }
    public DateTime Date { get; set; }
    public long MealId { get; set; }
    public string MealTitle { get; set; }
    public int CookingCapacity { get; set; }
    public int QuotaCount { get; set; }
    public int ReservedCount { get; set; }
    public int DeliveredCount { get; set; }
    public int RemainingCount => Math.Max(QuotaCount - ReservedCount, 0);
    public decimal CapacityUsage => CookingCapacity <= 0
        ? 0
        : Math.Round((decimal)ReservedCount * 100 / CookingCapacity, 1);
    public bool IsOverCapacity =>
        CookingCapacity > 0 && ReservedCount > CookingCapacity;
}
