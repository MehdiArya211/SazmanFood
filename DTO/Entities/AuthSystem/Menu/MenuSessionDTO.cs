using DTO.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text;

namespace DTO.Menu
{
    /// <summary>
    /// اطلاعات منو که به برای هر کاربر درون سشن قرار میگیرد
    /// </summary>
    public class MenuSessionDTO : EntityDTO
    {
        [Display(Name = "عنوان منو")]
        public string Title { get; set; }


        /// <summary>
        /// شناسه منو سر دسته
        /// </summary>
        [Display(Name = "سر دسته")]
        public long? ParentId { get; set; }



        /// <summary>
        /// ترتیب نمایش منو ها
        /// </summary>
        [Display(Name = "ترتیب")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int Order { get; set; }


        [Display(Name = "عنوان آیکن متریال")]
        public string Icon { get; set; }


        /// <summary>
        /// در لیست منوها نمایش داده شود یا صرفا
        /// یک اکشن است که دسترسی برای آن تعیین می شود
        /// </summary>
        [Display(Name = "نمایش در منو")]
        public bool ShowInMenu { get; set; }
                

        /// <summary>
        /// آیا منو درج شده لینک دارد یا خیر.
        /// منو های سر دسته لینک ندارند
        /// </summary>
        [Display(Name = "لینک دارد")]
        public bool HasLink { get; set; }


        /// <summary>
        /// آیا دسترسی به منو مربوطه نیاز به احراز هویت مجدد دارد؟
        /// </summary>
        [Display(Name = "نیاز به احراز هویت مجدد دارد؟")]
        public bool NeedReAuthorize { get; set; }



        #region پروپرتی های مربوط به ایجاد لینک منو
        /// <summary>
        /// نام اریایی که با کلیک بر روی منو باید اجرا شود
        /// </summary>
        [Display(Name = "اریا")]
        public string Area { get; set; }


        /// <summary>
        /// نام کنترلری که با کلیک بر روی منو باید اجرا شود
        /// </summary>
        [Display(Name = "کنترلر")]
        public string Controller { get; set; }


        /// <summary>
        /// نام اکشنی که با کلیک بر روی منو باید اجرا شود
        /// </summary>
        [Display(Name = "اکشن")]
        public string Action { get; set; }


        /// <summary>
        /// پارامترهای آدرس که به صورت جیسون از کاربر دریافت می شود
        /// و پس از تبدیل به رشته ذخیره می گردد
        /// </summary>
        [Display(Name = "پارامترهای آدرس")]
        public string Parameters { get; set; }



        /// <summary>
        /// پارامترها بصورت جیسون
        /// </summary>
        public object? Params
        {
            get
            {
                if (string.IsNullOrEmpty(Parameters))
                    return null;
                return JsonConvert.DeserializeObject<object>(Parameters);
            }
        }
        #endregion


        /// <summary>
        /// منو های زیر دسته
        /// </summary>
        public IEnumerable<MenuSessionDTO> SubMenus { get; set; }



        #region استخراج مدل تری ویو از منو
        public static Expression<Func<Domain.Entities.Menu, MenuSessionDTO>> Selector
        {
            get
            {
                return model => new MenuSessionDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                    ParentId = model.ParentId,
                    HasLink = model.HasLink,
                    Icon = model.MaterialIcon,
                    Order = model.Sort,
                    ShowInMenu = model.ShowInMenu,
                   // NeedReAuthorize= model.NeedReAuthorize,
                    Area = model.Area,
                    Controller = model.Controller,
                    Action = model.Action,
                    Parameters = model.Parameters
                };
            }
        }
        #endregion





        #region استخراج مدل تری ویو از RoleMenu
        public static Expression<Func<Domain.Entities.RoleMenu, MenuSessionDTO>> RoleMenuSelector
        {
            get
            {
                return model => new MenuSessionDTO()
                {
                    Id = model.Menu.Id,
                    Title = model.Menu.Title,
                    ParentId = model.Menu.ParentId,
                    HasLink = model.Menu.HasLink,
                    Icon = model.Menu.MaterialIcon,
                    Order = model.Menu.Sort,
                    ShowInMenu = model.Menu.ShowInMenu,
                    Area = model.Menu.Area,
                    //NeedReAuthorize = model.Menu.NeedReAuthorize,
                    Controller = model.Menu.Controller,
                    Action = model.Menu.Action,
                    Parameters = model.Menu.Parameters
                };
            }
        }
        #endregion



    }
}
