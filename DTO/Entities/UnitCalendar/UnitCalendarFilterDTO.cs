using Domain.Enums;

namespace DTO.Entities
{
    public class UnitCalendarFilterDTO
    {
        public int? OrgId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DayType? DayType { get; set; }
    }
}