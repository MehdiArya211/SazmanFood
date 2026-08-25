using BLL.Interface;
using Domain.Entities;
using DTO.Base;
using DTO.DataTable;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{

    /// <summary>
    /// مدیریت نقش ها
    /// </summary>
    public interface IRoleManager : IManager<Role, ApplicationContext>
    {

        /// <summary>
        /// گرفتن لیست نقش ها برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        DataTableResponseDTO<Role> GetDataTableDTO(DataTableSearchDTO searchData);



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
        IList<SelectListDTO> GetSelectListDTO(bool JustEnabled = false);



        /// <summary>
        /// ویرایش نقش به همراه منوهای قابل دسترسی
        /// </summary>
        /// <param name="role">نقش</param>
        /// <param name="menus">منوهای قابل دسترسی</param>
        /// <returns></returns>
        BaseResult UpdateWithMenus(Role role, List<RoleMenu> menus);



        /// <summary>
        /// حذف نقش به همراه دسترسی ها
        /// </summary>
        /// <returns></returns>
        BaseResult DeleteWithMenus(long id);

    }
}
