using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DTO.Base
{
    /// <summary>
    /// نمایش بصورت درختی
    /// </summary>
    public class TreeViewDTO
    {
        public long id { get; set; }
        public string text { get; set; }
        public long? parentId { get; set; }
        public IList<TreeViewDTO> nodes { get; set; }

    }
}
