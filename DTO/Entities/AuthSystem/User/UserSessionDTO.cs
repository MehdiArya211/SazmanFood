using Domain.Enums;
using DTO.Base;
using DTO.Menu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DTO.User
{
	/// <summary>
	/// اطلاعات کاربر که پس از لاگین درون سشن قرار میگیرد
	/// </summary>
	public class UserSessionDTO : EntityDTO
	{
		[Display(Name = "نام کاربری")]
		public string Username { get; set; }

		[Display(Name = "تلفن همراه")]
		public string Mobile { get; set; }

		[Display(Name = "نام")]
		public string FullName { get; set; }

		[Display(Name = "فعال است")]
		public bool IsEnabled { get; set; }


		[Display(Name = "نقش کاربر")]
		public long? RoleId { get; set; }
		public string Role { get; set; }


		[Display(Name = "نوع کاربر")]
		public UserType? Type { get; set; }


		public int OmdOrgId { get; set; }



		[Display(Name = "استان")]
		//[Required(ErrorMessage = "{0} الزامی است.")]
		public int? ProvinceId { get; set; }

		[Display(Name = "شهر")]
		//[Required(ErrorMessage = "{0} الزامی است.")]
		public int? CityId { get; set; }

		/// <summary>
		/// کاربر هر چند روز باید کلمه عبور خود را عوض کند
		/// </summary>
		[Display(Name = "دوره تغییر کلمه عبور")]
		public int? ChangePasswordCycle { get; set; }


		/// <summary>
		/// منوهای دسترسی کاربر بدون حالت درختی
		/// </summary>
		public IEnumerable<MenuSessionDTO> Menus { get; set; }


		[Display(Name = "کلمه عبور تغییر کرده است")]
		public bool PasswordIsChanged { get; set; }

        public long? PersonId { get; set; }
        public int? PersonCode { get; set; }
        public int? NationalCode { get; set; }
        public long? OrganGarrisonId { get; set; }


        #region سلکتور
        /// <summary>
        /// فیلدهای لازم برای استخراج مدل از موجودیت مربوطه
        /// </summary>
        public static Expression<Func<Domain.Entities.User, UserSessionDTO>> Selector
		{
			get
			{
				return model => new UserSessionDTO()
				{
					Id = model.Id,
					CityId = model.CityId,
					ProvinceId = model.ProvinceId,
					Mobile = model.Mobile,
					FullName = model.Name,
					IsEnabled = model.IsEnabled,
					Username = model.Username,
					RoleId = model.RoleId,
					Role = model.Role.Title,
					Type = model.Type,
					ChangePasswordCycle = model.ChangePasswordCycle,
					PasswordIsChanged = model.PasswordIsChanged,
					OmdOrgId = model.OmdOrgId,
					PersonId = model.PersonId,
					PersonCode = model.PersonCode,
					NationalCode = model.NationalCode,
					OrganGarrisonId = model.OrganGarrisonId,
					Menus = model.Role.Menus.AsQueryable()
									   .Where(x => x.Menu.IsEnabled /*&& x.Menu.ForCustomer == model.IsCustomer*/)
									   .OrderBy(x => x.Menu.Sort)
									   .Select(MenuSessionDTO.RoleMenuSelector).AsEnumerable()
				};
			}
		}
		#endregion


		#region توابع
		/// <summary>
		/// آیا کاربر به منوی خاصی دسترسی دارد؟
		/// </summary>
		/// <param name="Id">آیدی منو</param>
		/// <returns></returns>
		public bool HasMenu(long Id)
		{
			return Menus != null && Menus.Any(x => x.Id == Id);
		}

		/// <summary>
		/// آیا کاربر به منوی خاصی دسترسی دارد؟
		/// </summary>
		/// <returns></returns>
		public bool HasMenu(string Area, string Controller, string Action)
		{
			Area = Area?.Trim().ToLower();
			Controller = Controller?.Trim().ToLower();
			Action = Action?.Trim().ToLower();
			return Menus != null && Menus.Any(x => x.Area == Area && x.Controller == Controller && x.Action == Action);
		}

		/// <summary>
		/// آیا کاربر به منو کامنت ها دسترسی دارد؟
		/// </summary>
		/// <returns></returns>
		public bool HasCommentMenu()
		{
			return HasMenu("crm", "comments", "index");
		}


		/// <summary>
		/// آیا کاربر به منو تماس با ما دسترسی دارد؟
		/// </summary>
		/// <returns></returns>
		public bool HasContactUsMenu()
		{
			return HasMenu("crm", "contactus", "index");
		}


		/// <summary>
		/// آیا کاربر به منو تیکت ها دسترسی دارد؟
		/// </summary>
		/// <returns></returns>
		public bool HasTicketMenu()
		{
			return HasMenu("ticketsystem", "tickets", "index");
		}


		/// <summary>
		/// آیا کاربر به منو سفارشات سوپروایزر دسترسی دارد؟
		/// </summary>
		/// <returns></returns>
		public bool HasSupervisorOrdersMenu()
		{
			return HasMenu("shoppingsystem", "supervisororders", "index");
		}
		#endregion

	}
}
