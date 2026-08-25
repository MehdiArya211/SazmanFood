using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FajrLog.Enum
{
    /// <summary>
    /// وضعیت عملیات
    /// </summary>
    public enum FajrActionFlag
    {
        Success = 0,
        UnSuccess = 1,
        Fail = 2,
        Allow = 3,
        Deny = 4
    }
}
