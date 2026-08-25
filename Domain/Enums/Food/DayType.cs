using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums;

public enum DayType
{
    [Description("عادی")]
    Normal = 1,

    [Description("تعطیل")]
    Holiday = 2,

    [Description("نیمه تعطیل")]
    HalfHoliday = 3
}
