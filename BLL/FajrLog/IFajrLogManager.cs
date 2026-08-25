using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.FajrLog
{
    public interface IFajrLogManager
    {
        void SendLogFajr(string ipAddressFajr, string portNumFajr, string clientHostName, string clientIp, string softwareId, string pageTitle,
            string url, string username, string userId, string actionType, string sensitivity, bool isSuccess, string subType,
            string subTypeDec);
    }
}
