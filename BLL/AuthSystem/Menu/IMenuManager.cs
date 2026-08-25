using Domain.Entities;
using DTO.Base;
using DTO.Menu;
using Infrastructure.Data;

namespace BLL.Interface
{
    /// <summary>
    /// مدیریت منو ها
    /// </summary>
    public interface IMenuManager : IManager<Menu, ApplicationContext>
    {
        /// <summary>
        /// گرفتن اطلاعات منو بصورت درختی
        /// </summary>
        /// <param name="JustEnabled"></param>
        /// <returns></returns>
        List<MenuTreeViewDTO> GetTreeViewDTO(bool JustEnabled = false);



        /// <summary>
        ///  داده خامی که برای ساختار درختی دریافت شده است
        ///  به صورت ساختار درختی واقعی در می آید
        /// </summary>
        /// <param name="data">داده خام خارج از ساختار درختی</param>
        /// <param name="parentId">آیدی والد</param>
        /// <returns></returns>
        List<MenuTreeViewDTO> ReshapeTreeViewData(List<MenuTreeViewDTO> data, long? parentId = null);




        /// <summary>
        /// آیا این منو، زیر منو دارد؟
        /// </summary>
        /// <param name="ParentId">آیدی سردسته</param>
        /// <returns></returns>
        bool HasChild(long? ParentId);



        /// <summary>
        /// حذف منو به همراه برداشتن دسترسی کاربران و نقش هایی که به آن دسترسی دارند
        /// </summary>
        /// <param name="id">ایدی منو</param>
        /// <returns></returns>
        BaseResult DeleteWithRoles(long id);

    }
}
