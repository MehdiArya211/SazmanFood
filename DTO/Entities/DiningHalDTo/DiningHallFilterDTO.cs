using Domain.Enums.Food;

namespace DTO.Entities.DiningHalDTo
{
    public class DiningHallFilterDTO
    {
        public string Title { get; set; }

        public int? OrgId { get; set; }

        public long? PersonalTypeId { get; set; }

        public DiningHallUsageType? UsageType { get; set; }

        public DiningHallSupplyType? SupplyType { get; set; }

        public bool? IsActive { get; set; }
    }
}