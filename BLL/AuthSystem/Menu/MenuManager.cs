using BLL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DTO.Base;
using DTO.Menu;
using LinqKit;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace BLL
{
    /// <summary>
    /// مدیریت منو ها
    /// </summary>
    public class MenuManager : Manager<Menu, ApplicationContext>, IMenuManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession Session;


        public MenuManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor) : base(_Context, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }



        /// <summary>
        /// گرفتن لیست منو ها برای تری ویو
        /// </summary>
        public List<MenuTreeViewDTO> GetTreeViewDTO(bool JustEnabled = false)
        {
            var filter = PredicateBuilder.New<Menu>(true);

            if (JustEnabled)
                filter = filter.And(x => x.IsEnabled);

            return UOW.Menus.GetDTO<MenuTreeViewDTO>(MenuTreeViewDTO.Selector, filter, x => x.OrderBy(f => f.Sort).ThenBy(f => f.Id)).ToList();
        }




        /// <summary>
        ///  داده خامی که برای ساختار درختی دریافت شده است
        ///  به صورت ساختار درختی واقعی در می آید
        /// </summary>
        /// <param name="data">داده خام خارج از ساختار درختی</param>
        /// <param name="parentId">آیدی والد</param>
        /// <returns></returns>
        public List<MenuTreeViewDTO> ReshapeTreeViewData(List<MenuTreeViewDTO> data, long? parentId = null)
        {
            if (data == null || data.Count() == 0)
                return null;

            var model = data.Where(x => x.parentId == parentId).ToList();

            if (model == null || model.Count() == 0)
                return null;

            foreach (var item in model)
                item.nodes = ReshapeTreeViewData(data, item.id);

            return model;
        }



        /// <summary>
        /// افزودن منو جدید
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override BaseResult Create(Menu entity)
        {
            var isValid = Validate(entity);

            if (!isValid.Status)
                return isValid;

            if (!entity.HasLink)
            {
                entity.Area = null;
                entity.Controller = null;
                entity.Action = null;
                entity.Parameters = null;
            }
            else
            {
                entity.Area = entity.Area?.ToLower();
                entity.Controller = entity.Controller?.ToLower();
                entity.Action = entity.Action?.ToLower();
            }

            return base.Create(entity);
        }



        /// <summary>
        /// ویرایش منو
        /// </summary>
        /// <param name="entity">منو</param>
        /// <returns></returns>
        public override BaseResult Update(Menu entity)
        {
            var isValid = Validate(entity);

            if (!isValid.Status)
                return isValid;

            var menu = GetById(entity.Id);
            bool NeedReAuthorize = menu.NeedReAuthorize;

            menu.Title = entity.Title;
            menu.HasLink = entity.HasLink;
            menu.ShowInMenu= entity.ShowInMenu;
            menu.Description = entity.Description;
            menu.IsEnabled = entity.IsEnabled;
            menu.NeedReAuthorize= entity.NeedReAuthorize;
            menu.MaterialIcon = entity.MaterialIcon;
            menu.Sort = entity.Sort;
            if (!entity.HasLink)
            {
                menu.Area = null;
                menu.Controller = null;
                menu.Action = null;
                menu.Parameters = null;
            }
            else
            {
                menu.Area = entity.Area?.ToLower();
                menu.Controller = entity.Controller?.ToLower();
                menu.Action = entity.Action?.ToLower();
            }
            var res = base.Update(menu);
            res.Model = NeedReAuthorize;
            return res;
        }




        /// <summary>
        /// اعتبار سنجی منو
        /// </summary>
        /// <param name="menu">منو</param>
        /// <returns></returns>
        public BaseResult Validate(Menu menu)
        {
            if (menu.HasLink)
            {
                if (string.IsNullOrEmpty(menu.Controller))
                    return new BaseResult { Status = false, Message = "کنترلر را وارد کنید!" };
                if (string.IsNullOrEmpty(menu.Action))
                    return new BaseResult { Status = false, Message = "اکشن را وارد کنید!" };
            }
            return new BaseResult { Status = true };
        }




        /// <summary>
        /// آیا این منو، زیر منو دارد؟
        /// </summary>
        /// <param name="ParentId">آیدی سردسته</param>
        /// <returns></returns>
        public bool HasChild(long? ParentId)
        {
            return UOW.Menus.Any(x => x.ParentId == ParentId);
        }





        /// <summary>
        /// حذف منو به همراه برداشتن دسترسی کاربران و نقش هایی که به آن دسترسی دارند
        /// </summary>
        /// <param name="id">ایدی منو</param>
        /// <returns></returns>
        public BaseResult DeleteWithRoles(long id)
        {
            var menu = UOW.Menus.FirstOrDefault(x => x.Id == id);
            UOW.Menus.Remove(menu);

            var roles = UOW.RoleMenus.Get(x => x.MenuId == id);
            UOW.RoleMenus.RemoveRange(roles);

            var IsSuccess = UOW.Commit();
            return new BaseResult
            {
                Status = IsSuccess,
                Message = IsSuccess ? "منو با موفقیت حذف شد." : "حذف منو با خطا همراه بوده است.",
                Model = menu.NeedReAuthorize
            };
        }

    }
}
