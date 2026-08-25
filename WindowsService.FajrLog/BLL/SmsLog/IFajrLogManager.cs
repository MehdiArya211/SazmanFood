using FajrLog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.FajrLog
{
    public interface IFajrLogManager
    {




        /// <summary>
        /// افزودن لاگ ها به دیتابیس 
        /// </summary>
        /// <returns></returns>
        bool CreateRange(List<FajrLogEntity> logs);

    }
}
