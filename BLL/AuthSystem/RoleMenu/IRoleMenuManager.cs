using Domain.Entities;
using DTO.Base;
using Infrastructure.Data;

namespace BLL.Interface
{
    /// <summary>
    /// مدیریت منوهای قابل دسترسی هر نقش
    /// </summary>
    public interface IRoleMenuManager : IManager<RoleMenu, ApplicationContext>
    {
        /// <summary>
        /// گرفتن همه دسترسی های یک نقش خاص
        /// </summary>
        /// <param name="RoleId">شناسه نقش</param>
        /// <returns></returns>
        List<RoleMenu> GetByRoleId(long? RoleId);


        /// <summary>
        /// حذف دسترسی های قبلی و افزودن دسترسی های جدید
        /// </summary>
        /// <param name="RoleId">شناسه نقش</param>
        /// <param name="MenuIds">شناسه منو ها</param>
        /// <returns></returns>
        BaseResult UpdateRoleMenus(long RoleId, List<long> MenuIds);

    }
}
