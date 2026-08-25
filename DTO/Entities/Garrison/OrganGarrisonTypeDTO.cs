using System.ComponentModel.DataAnnotations;

namespace DTO
{
	public class OrganGarrisonTypeDTO
	{
		[Display(Name = "شناسه")]
		public long Id { get; set; }
		public string Title { get; set; }
		public int? Code { get; set; }
		public int? SortName { get; set; }
		public bool IsDeleted { get; set; }
	}
}
