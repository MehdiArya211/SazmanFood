using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extentions
{


    /// <summary>
    /// مدل برای تبدیل enum به کلاس
    /// </summary>
    public class EnumViewModel
    {
        public int? Id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
    }
}
