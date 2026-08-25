namespace DTO.Entities
{
    public class ExtraQuotaPersonDTO
    {
        public long Id { get; set; }

        public long RequestId { get; set; }

        public string RankTitle { get; set; }

        public string FullName { get; set; }

        public string PersonCode { get; set; }

        public string NationalCode { get; set; }

        public long? DiningHallId { get; set; }

        public string DiningHallTitle { get; set; }
    }
}