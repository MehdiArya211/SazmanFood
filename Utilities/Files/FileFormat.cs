using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extentions
{
    /// <summary>
    /// فرمت فایل ها . مثلا ص.تی، تصویری 
    /// </summary>
    public enum FileFormat
    {

        [Description("تصویر")]
        Image = 0,

        [Description("پی دی اف")]
        Pdf = 1,

        [Description("وُرد")]
        Docx = 2,

        [Description("اکسل")]
        Excel = 3,

        [Description("پاور پوینت")]
        Powerpoint = 4,

        [Description("زیپ")]
        Zip = 5,

        [Description("فیلم")]
        Video = 6,

        [Description("صوتی")]
        Audio = 7,

        [Description("سایر")]
        Other = 8,

        [Description("نرم افزار اندروید")]
        Apk = 9,
    }
}
