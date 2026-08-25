namespace DTO.Entities
{
    public class UnitQuotaMealItemDTO
    {
        public long UnitQuotaId { get; set; }
        public long UnitStatisticId { get; set; }
        public int OrgId { get; set; }
        public string OrgTitle { get; set; }
        public long PersonalTypeId { get; set; }
        public string PersonalTypeTitle { get; set; }
        public long YeganTypeId { get; set; }
        public string YeganTypeTitle { get; set; }
        public Domain.Enums.DayType DayType { get; set; }
        public long MealId { get; set; }
        public string MealTitle { get; set; }
        public int QuotaCount { get; set; }
    }
}