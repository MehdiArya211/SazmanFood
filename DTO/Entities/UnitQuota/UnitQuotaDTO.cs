using Domain.Enums;

namespace DTO.Entities
{
    public class UnitQuotaDTO
    {
        public int Row { get; set; }
        public int OrgId { get; set; }
        public string OrgTitle { get; set; }
        public long YeganTypeId { get; set; }
        public string YeganTypeTitle { get; set; }
        public long MealId { get; set; }
        public string MealTitle { get; set; }
        public DayType DayType { get; set; }
        public string DayTypeTitle { get; set; }
        public int TotalQuotaCount { get; set; }
        public long? OfficialUnitQuotaId { get; set; }
        public int OfficialQuotaCount { get; set; }
        public int OfficialRegisteredCount { get; set; }
        public long? DutyUnitQuotaId { get; set; }
        public int DutyQuotaCount { get; set; }
        public int DutyRegisteredCount { get; set; }

        public int OfficialRemainingCount
        {
            get
            {
                return Math.Max(
                    0,
                    OfficialQuotaCount -
                    OfficialRegisteredCount);
            }
        }

        public int DutyRemainingCount
        {
            get
            {
                return Math.Max(
                    0,
                    DutyQuotaCount -
                    DutyRegisteredCount);
            }
        }
    }
}