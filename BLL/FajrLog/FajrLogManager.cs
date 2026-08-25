using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;


namespace BLL.FajrLog
{
    public class FajrLogManager : IFajrLogManager
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public void SendLogFajr(string ipAddressFajr, string portNumFajr, string clientHostName, string clientIp, string softwareId, string pageTitle,
            string url, string username, string userId, string actionType, string sensitivity, bool isSuccess, string subType,
            string subTypeDec)
        {
			//try
			//{
                long epoch = ConvertToTimestamp(DateTime.Now);
                var description = new object();
                if (string.IsNullOrWhiteSpace(subTypeDec.ToString()))
                {
                   
                }
                else
                {

                    JToken jcheck = JToken.Parse(subTypeDec.ToString().Trim());
                    description = JsonConvert.DeserializeObject(subTypeDec.ToString().Trim());

                }
                LogReport logreport = new LogReport()
                {
                    Time = epoch.ToString(),
                    SoftwareID = softwareId.Trim(),
                    SoftwareName = "LearnSkill".Trim(),
                    SoftwareVersion = "1.0.0".Trim(),
                    ServerHostname = "NezajaWebSite".Trim(),
                    ServerIp = "10.128.155.213".Trim(),
                    PortNumber = "80".Trim(),
                    ClientHostname = clientHostName.Trim(),
                    ClientIP = clientIp.Trim(),
                    PageTitle = pageTitle.Trim(),
                    URL = url.Trim(),
                    Username = username.Trim(),
                    UserUniqueID = userId.Trim(),
                    ActionType =actionType.Trim(),
                    Sensitivity = sensitivity.Trim(),
                    Importance = "2".Trim(),
                    Flag = isSuccess? "success" : "faild",
                    SubType = subType.Trim(),
                    SubTypeDescription = description
                };
                string json = JsonConvert.SerializeObject(logreport, Formatting.Indented);

                byte[] bytes = new byte[1024];
                Queue<string> queueLog = new Queue<string>();

                string ipAd = ipAddressFajr;
                var ipAddress = IPAddress.Parse(ipAd);
                string portNumber = portNumFajr;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32(portNumber));

                // Create a TCP/IP  socket.    
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Encode the data string into a byte array.   
                var utf8 = new UTF8Encoding();
                byte[] msg = utf8.GetBytes(json);

                // Connect to Remote EndPoint  
                sender.Connect(remoteEP);

                // Send the data through the socket.    
                var bytesSent = sender.Send(msg);

   //         }
			//catch (Exception e)
			//{

			//	throw;
			//}
        }
        private static long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }
        public class LogReport
        {
            public string Time;
            public string SoftwareID;
            public string SoftwareName;
            public string SoftwareVersion;
            public string ServerHostname;
            public string ServerIp;
            public string PortNumber;
            public string ClientHostname;
            public string ClientIP;
            public string PageTitle;
            public string URL;
            public string Username;
            public string UserUniqueID;
            public string ActionType;
            public string Sensitivity;
            public string Importance;
            public string Flag;
            public string SubType;
            //public String SubTypeDescription;
            //public Description SubTypeDescription;
            public object SubTypeDescription;
        }
    }
}
