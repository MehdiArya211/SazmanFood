using Domain.Entities;
using Domain.Entities.FoodManage;
using Domain.Entities.Garrison;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace DTO.Base
{
    /// <summary>
    /// مدل برای دراپدون
    /// </summary>
    public class SelectListDTO
    {
        [Display(Name = "شناسه")]
        public long Id { get; set; }
        public long? ParentId { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }



        #region سازنده ها
        public SelectListDTO()
        {
        }

        public SelectListDTO(long id, string title)
        {
            Id = id;
            ParentId = ParentId;
            Title = title;
        }
        #endregion




        #region تبدیل نقش ها به مدل دراپدون
        public static Expression<Func<Domain.Entities.Role, SelectListDTO>> RoleSelector
        {
            get
            {
                return model => new SelectListDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                };
            }
        }
        #endregion


        #region تبدیل کاربران به مدل دراپدون
        public static Expression<Func<Domain.Entities.User, SelectListDTO>> UserSelector
        {
            get
            {
                return model => new SelectListDTO()
                {
                    Id = model.Id,
                    Title = model.Name,
                };
            }
        }
		#endregion

		#region تبدیل پادگان ها به مدل دراپدون
		public static Expression<Func<OrganGarrison, SelectListDTO>> OrganGarrisonSelector
		{
			get
			{
				return model => new SelectListDTO()
				{
					Id = model.Id,
					ParentId = model.ParentId,
					Title = model.Title,
				};
			}
		}
		#endregion

		#region تبدیل روز های هفته  به مدل دراپدون
		public static Expression<Func<Domain.Entities.Days, SelectListDTO>> DaySelector
		{
			get
			{
				return model => new SelectListDTO()
				{
					Id = model.Id,
					Title = model.Title,
				};
			}
		}
        #endregion
        #region تبدیل غذا  به مدل دراپدون
        public static Expression<Func<Foods, SelectListDTO>> FoodsSelector
        {
            get
            {
                return model => new SelectListDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                };
            }
        }
        #endregion

        #region تبدیل وعده به مدل دراپدون
        public static Expression<Func<Domain.Entities.Meal, SelectListDTO>> MealSelector
		{
			get
			{
				return model => new SelectListDTO()
				{
					Id = model.Id,
					Title = model.Title,
				};
			}
		}
		#endregion

		#region تبدیل نوع پرسنل به مدل دراپدون
		public static Expression<Func<Domain.Entities.PersonalType, SelectListDTO>> PersonalTypeSelector
		{
			get
			{
				return model => new SelectListDTO()
				{
					Id = model.Id,
					Title = model.Title,
				};
			}
		}
        #endregion
        #region تبدیل نوع یگان به مدل دراپدون
        public static Expression<Func<Domain.Entities.YeganType, SelectListDTO>> YeganTypeSelector
        {
            get
            {
                return model => new SelectListDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                };
            }
        }
        #endregion

        #region تبدیل نوع غذا به مدل دراپدون
        public static Expression<Func<Domain.Entities.FoodManage.FoodTypes, SelectListDTO>> FoodTypesSelector
        {
            get
            {
                return model => new SelectListDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                };
            }
        }
        #endregion

        #region تبدیل سال   به مدل دراپدون
        public static Expression<Func<Years, SelectListDTO>> YearsSelector
        {
            get
            {
                return model => new SelectListDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                };
            }
        }
        #endregion

        #region تبدیل فصل  به مدل دراپدون
        public static Expression<Func<Season, SelectListDTO>> SeasonSelector
        {
            get
            {
                return model => new SelectListDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                };
            }
        }
        #endregion


    }

}
