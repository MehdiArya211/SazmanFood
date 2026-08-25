using DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DTO.Menu
{
    /// <summary>
    /// نمایش منو ها بصورت درختی
    /// </summary>
    public class MenuTreeViewDTO: TreeViewDTO
    {
        public string title { get; set; }
        public bool isEnabled { get; set; }
        public bool hasLink { get; set; }
        public string icon { get; set; }
        public new IList<MenuTreeViewDTO> nodes { get; set; }


        #region استخراج مدل تری ویو از منو
        public static Expression<Func<Domain.Entities.Menu, MenuTreeViewDTO>> Selector
        {
            get
            {
                return model => new MenuTreeViewDTO()
                {
                    id = model.Id,
                    text = model.Title,
                    title = model.Title,
                    parentId = model.ParentId,
                    isEnabled = model.IsEnabled,
                    hasLink = model.HasLink,
                    icon = model.MaterialIcon
                };
            }
        }
        #endregion


    }
}
