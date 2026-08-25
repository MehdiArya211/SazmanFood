using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum CourseStatus
    {
        [Description("خاتمه یافته")]
        Khatemeyafte = 1,

        [Description("در حال اجرا")]
        darhaleejra = 2,
    }
}
