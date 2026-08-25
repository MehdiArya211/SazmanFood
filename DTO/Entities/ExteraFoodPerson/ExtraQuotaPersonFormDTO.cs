namespace DTO.Entities
{
    public class ExtraQuotaPersonFormDTO
    {
        public long RequestId { get; set; }

        public int OrgId { get; set; }

        public string OrgTitle { get; set; }

        public long MealId { get; set; }

        public string MealTitle { get; set; }

        public long PersonalTypeId { get; set; }

        public string PersonalTypeTitle { get; set; }

        public int Count { get; set; }

        public int RegisteredCount { get; set; }

        public int RemainingCount
        {
            get
            {
                return Math.Max(0, Count - RegisteredCount);
            }
        }

        public bool IsOfficial { get; set; }

        public bool IsDuty { get; set; }

        public List<ExtraQuotaPersonDTO> Persons { get; set; } = new();
    }
}