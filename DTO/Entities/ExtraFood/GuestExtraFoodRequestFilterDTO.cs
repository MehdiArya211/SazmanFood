using Domain.Enums;

namespace DTO.Entities
{
    public class GuestExtraFoodRequestFilterDTO0
    {
        public int? OrgId { get; set; }

        public long? MealId { get; set; }

        public long? PersonalTypeId { get; set; }

        public long? YeganTypeId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public GuestExtraFoodRequestStatus? Status { get; set; }
    }
}