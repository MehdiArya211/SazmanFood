using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DTO
{
	public class OrganGarrisonDTO
	{
		[Display(Name = "شناسه")]
		public long Id { get; set; }
		public string Title { get; set; }
		public long? ParentId { get; set; }
		public long OrganGarrisonTypeId { get; set; }
		public string OrganGarrisonTypeTitle { get; set; }
		public string ParentTitle { get; set; }
		public int? OrgId { get; set; }
		public int? Code { get; set; }
		public int? SortName { get; set; }
		public bool IsDeleted { get; set; }
        public int EstedadKadr { get; set; }
        public int EstedadVazife { get; set; }

        //hiiiiii
    }

	public class OrganGarrisonDataTableDTO : OrganGarrisonDTO
	{
		public static Expression<Func<Domain.Entities.Garrison.OrganGarrison, OrganGarrisonDataTableDTO>> Selector
		{
			get
			{
				return model => new OrganGarrisonDataTableDTO()
				{
					Id = model.Id,
					Title = model.Title,
					ParentId = model.ParentId,
					OrganGarrisonTypeId = model.OrganGarrisonTypeId,
					Code = model.Code,
					SortName = model.SortName,
					OrganGarrisonTypeTitle = model.OrganGarrisonType.Title,
					IsDeleted = model.IsDeleted,
					EstedadKadr = model.EstedadKadr,
					EstedadVazife = model.EstedadVazife


				};
			}
		}
	}

	public class OrganGarrisonCreateDTO 
	{
		public string Title { get; set; }
		public long? ParentId { get; set; }
		public long OrganGarrisonTypeId { get; set; }

		public int? Code { get; set; }
		public int? SortName { get; set; }
		public bool IsDeleted { get; set; }
	}

	public class OrganGarrisonEditDTO : OrganGarrisonDTO
	{
		public static Expression<Func<Domain.Entities.Garrison.OrganGarrison, OrganGarrisonEditDTO>> Selector
		{
			get
			{
				return model => new OrganGarrisonEditDTO()
				{
					Id = model.Id,
					Title = model.Title,
					ParentId = model.ParentId,
					OrganGarrisonTypeId = model.OrganGarrisonTypeId,
					Code = model.Code,
					SortName = model.SortName,
					IsDeleted = model.IsDeleted,


				};
			}
		}
	}
}
