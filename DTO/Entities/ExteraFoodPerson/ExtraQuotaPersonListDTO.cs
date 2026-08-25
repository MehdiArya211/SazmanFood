using Utilities.Extentions;

namespace DTO.Entities
{
    public class ExtraQuotaPersonListDTO
    {
        public int Row { get; set; }

        public long RequestId { get; set; }

        public int OrgId { get; set; }

        public string OrgTitle { get; set; }

        public long MealId { get; set; }

        public string MealTitle { get; set; }

        public long PersonalTypeId { get; set; }

        public string PersonalTypeTitle { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Count { get; set; }

        public int RegisteredCount { get; set; }

        public string FromDateFa =>
            FromDate.ToPersianDateTime().ToString();

        public string ToDateFa =>
            ToDate.ToPersianDateTime().ToString();
    }
}