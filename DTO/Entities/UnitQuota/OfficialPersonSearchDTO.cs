namespace DTO.Entities
{
    public class OfficialPersonSearchDTO
    {
        public long PersonId { get; set; }
        public int PersonCode { get; set; }
        public string NationalCode { get; set; }
        public string RankTitle { get; set; }
        public string FullName { get; set; }
    }
}