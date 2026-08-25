using BLL.Interface;
using Domain.Entities;
using DTO.Base;
using DTO.DataTable;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BLL
{

	/// <summary>
	/// مدیریت نقش ها
	/// </summary>
	public class RoleManager : Manager<Role, ApplicationContext>, IRoleManager
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly ISession Session;


		public RoleManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor) : base(_Context, httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
			Session = httpContextAccessor.HttpContext.Session;
		}




		/// <summary>
		/// گرفتن لیست نقشها برای نمایش در پنل مدیریت
		/// </summary>
		/// <returns></returns>
		public DataTableResponseDTO<Role> GetDataTableDTO(DataTableSearchDTO searchData)
		{
			return UOW.Roles.GetDataTableDTO(searchData);
		}




		/// <summary>
		/// گرفتن لیست نقش ها در غالب دراپدون
		/// </summary>
		/// <param name="JustEnabled">
		/// تنها نقش های فعال لود شوند یا همه نقش ها؟
		/// <para>
		/// به صورت پیشفرض تنها نقش های فعال لود می شوند
		/// </para>
		/// </param>
		/// <returns></returns>
		public IList<SelectListDTO> GetSelectListDTO(bool JustEnabled = false)
		{
			if (JustEnabled)
				return UOW.Roles.GetDTO<SelectListDTO>(SelectListDTO.RoleSelector, x => x.IsEnabled).ToList();
			else
				return UOW.Roles.GetDTO<SelectListDTO>(SelectListDTO.RoleSelector).ToList();
		}





		/// <summary>
		/// ویرایش نقش به همراه منوهای قابل دسترسی
		/// </summary>
		/// <param name="role">نقش</param>
		/// <param name="menus">منوهای قابل دسترسی</param>
		/// <returns></returns>
		public BaseResult UpdateWithMenus(Role role, List<RoleMenu> menus)
		{
			var oldRole = UOW.Roles.FirstOrDefault(x => x.Id == role.Id, null, x => x.Include(z => z.Menus));
			if (oldRole.Menus != null && oldRole.Menus.Count() > 0)
			{
				UOW.RoleMenus.RemoveRange(oldRole.Menus);
				oldRole.Menus.Clear();
			}
			oldRole.Menus = menus;
			oldRole.Title = role.Title;
			oldRole.IsEnabled = role.IsEnabled;

			UOW.Roles.Update(oldRole);
			var isSuccess = UOW.Commit();
			return new BaseResult
			{
				Status = isSuccess,
				Message = isSuccess ? "تغییرات ذخیره شد." : "ثبت اطلاعات با خطا همراه بوده است!"
			};
		}




		/// <summary>
		/// حذف نقش به همراه دسترسی ها
		/// </summary>
		/// <returns></returns>
		public BaseResult DeleteWithMenus(long id)
		{
			var role = UOW.Roles.FirstOrDefault(x => x.Id == id, null, x => x.Include(z => z.Users));
			role.Users.Clear();
			UOW.Roles.Remove(role);

			var menus = UOW.RoleMenus.GetByRoleId(id);
			UOW.RoleMenus.RemoveRange(menus);

			var IsSuccess = UOW.Commit();
			return new BaseResult
			{
				Status = IsSuccess,
				Message = IsSuccess ? "نقش مورد نظر حذف شد." : "حذف نقش با خطا همراه بوده است. ابتدا از اینکه نقش را به کاربری اختصاص نداده اید مطمئن شوید."
			};
		}


	}
}
