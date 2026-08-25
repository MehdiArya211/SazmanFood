using System.ComponentModel.DataAnnotations;

namespace DTO
{
	public class StatisticOrganDTO
	{
		public long Id { get; set; }
		[Display(Name = "شناسه یگان پادگان")]
		public long OrganGarrisonId { get; set; }

		[Display(Name = "تعداد کادری")]
		public int? OfficerCount { get; set; }
		[Display(Name = "تعداد سربازان")]

		public int? SolidierCount { get; set; }
	}
}
