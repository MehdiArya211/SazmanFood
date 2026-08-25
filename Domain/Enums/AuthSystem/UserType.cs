using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    /// <summary>
    /// نوع کاربر
    /// </summary>
    public enum UserType
    {
        [Description("سایر")]
        Other = 0,

        [Description("سوپر ادمین")]
        SuperAdmin = 1
    }
}
