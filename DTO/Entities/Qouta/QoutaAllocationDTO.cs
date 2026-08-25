using DTO.User;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Utilities.Extentions;

namespace DTO
{
    public class QoutaPersonInfoDTO
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string PersonalCode { get; set; }
        public string FoodTitle { get; set; }
    }


    public class QoutaAllocationDTO
	{
		public long Id { get; set; }
		[Display(Name = "  پادگان")]
		public long? OrganGarrisonParentId { get; set; }
        [Display(Name = " یگان پادگان")]
        public long? OrganGarrisonId { get; set; }

		public string OrganGarrisonTitle { get; set; }
		public string OrganGarrisonParentTitle { get; set; }

		[Display(Name = " یگان")]
		public long? OrgId { get; set; }

		[Display(Name = "نام یگان")]
		[MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
		public string OrgTitle { get; set; }

		[Display(Name = "تاریخ سهمیه بندی")]
		[Required(ErrorMessage = "{0} الزامی است.")]
		public DateTime QoutaAllocationDate { get; set; }
        public string QoutaAllocationDateFa => QoutaAllocationDate.ToPersianDateTime().ToString();
        public int? QoutaAllocationDateInt { get { return QoutaAllocationDate.ToPersianDateTime().ToShortDateInt(); } }


        [Display(Name = "شناسه روز")]
		public long DayId { get; set; }

		[Display(Name = "روز")]
		[MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
		public string DayTitle { get; set; }

		[Display(Name = "شناسه وعده ی غذایی")]
		public long MealId { get; set; }

		[Display(Name = "وعده غذایی")]
		[MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
		public string MealTitle { get; set; }

        public string FoodTitle { get; set; } // ✅ غذا اضافه شد


    }
    public class QoutaAllocationDataTableDTO: QoutaAllocationDTO
	{
        public List<QoutaPersonInfoDTO> PersonList { get; set; } = new List<QoutaPersonInfoDTO>();


        public static Expression<Func<Domain.Entities.QoutaAllocation, QoutaAllocationDataTableDTO>> Selector
		{
			get
			{
				return model => new QoutaAllocationDataTableDTO()
				{
					Id = model.Id,
					OrganGarrisonId = model.OrganGarrisonId,
					OrganGarrisonTitle = model.OrganGarrison.Title,
                    OrganGarrisonParentTitle = model.OrganGarrison.Parent != null
                                       ? model.OrganGarrison.Parent.Title
                                       : null, 
                    OrgId = model.OrgId,
                    OrgTitle = model.OrgTitle,
					QoutaAllocationDate = model.QoutaAllocationDate,
					DayId = model.DayId,
					DayTitle = model.Day.Title,
					MealId = model.MealId,
					MealTitle = model.Meal.Title,
                   // FoodTitle = model.FoodPlanDay.Food.Title != null ? model.FoodPlanDay.Food.Title : null
					     // ✅ اضافه کردن نام غذا، با ایمن سازی
                FoodTitle = model.FoodPlanDay != null && model.FoodPlanDay.Food != null
                            ? model.FoodPlanDay.Food.Title
                            : "ثبت نشده"
,
                    PersonList = model.QoutaPerson
                                  .Select(p => new QoutaPersonInfoDTO
                                  {
                                      FName = p.FName,
                                      LName = p.LName,
                                      PersonalCode = p.PersonalCode,
                                      FoodTitle = p.MainFood != null ? p.MainFood.Title : "ثبت نشده"
                                  })
                                  .ToList()
                
            };
			}
		}
	}
	public class QoutaAllocationCreateDTO1		
	{
		[Display(Name = "  پادگان")]
		public long? OrganGarrisonParentId { get; set; }
        [Display(Name = "  یگان پادگان")]

        public long? OrganGarrisonId { get; set; }

		[Display(Name = " یگان")]
		public long? OrgId { get; set; }

		[Display(Name = "نام یگان")]
		[MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
		public string OrgTitle { get; set; }

		[Display(Name = "تاریخ سهمیه بندی")]
		[Required(ErrorMessage = "{0} الزامی است.")]
		public DateTime QoutaAllocationDate { get; set; }



        [Display(Name = " روز")]
		public long DayId { get; set; }

		[Display(Name = "روز")]
		[MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
		public string DayTitle { get; set; }

		[Display(Name = "شناسه وعده ی غذایی")]
		public long MealId { get; set; }

		[Display(Name = "وعده غذایی")]
		[MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
		public string MealTitle { get; set; }
        //[Display(Name = "نوع پرسنل (کادر / وظیفه)")]

        //public long PersonalTypeId { get; set; }

        //[Display(Name = "نام")]
        //[MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        //public string FName { get; set; }

        //[Display(Name = "نشان")]
        //[MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        //public string LName { get; set; }

        //[Display(Name = "کد پرسنلی")]
        //[MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        //public string PersonalCode { get; set; }
    }

	public class QoutaAllocationEditDTO: QoutaAllocationDTO
	{
		public static Expression<Func<Domain.Entities.QoutaAllocation, QoutaAllocationEditDTO>> Selector
		{
			get
			{
				return model => new QoutaAllocationEditDTO()
				{
					Id = model.Id,
					OrganGarrisonParentId = model.OrganGarrison.ParentId!=null
					?model.OrganGarrison.ParentId
					:null,
                    OrganGarrisonParentTitle = model.OrganGarrison.Parent != null
                                       ? model.OrganGarrison.Parent.Title
                                       : null,
                    OrganGarrisonId = model.OrganGarrisonId,
					OrgId = model.OrgId,
					OrgTitle = model.OrgTitle,

					QoutaAllocationDate = model.QoutaAllocationDate,
					DayId = model.DayId,
					DayTitle = model.DayTitle,
					MealId = model.MealId,
					MealTitle = model.MealTitle,

				};
			}
		}

	}

	public class QoutaAllocationFilterDataTableDTO
	{
		public long? OrganGarrisonId { get; set; }
		public long? OrganGarrisonParentId { get; set; }
		public DateTime? Date { get; set; }
		public long? DayId { get; set; }
		public long? MealId { get; set; }
		public long? PersonalType { get; set; }
		public long? UserId { get; set; }

	}
}
