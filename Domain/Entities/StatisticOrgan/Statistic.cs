using System.ComponentModel.DataAnnotations;
using Domain.Entities.Garrison;

namespace Domain.Entities
{
    /// <summary>
    /// آمار
    /// </summary>
    public class Statistic:EntityBase
	{
		[Display(Name = "شناسه یگان پادگان")]
		public long OrganGarrisonId { get; set; }

		[Display(Name = "تعداد کادری")]
		public int? OfficerCount { get; set; }
		[Display(Name = "تعداد سربازان")]

		public int? SolidierCount { get; set; }

		#region Relation
		public OrganGarrison OrganGarrison { get; set; }
		#endregion
	}
}
