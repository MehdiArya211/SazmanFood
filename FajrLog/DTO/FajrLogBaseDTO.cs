using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FajrLog.DTO
{
    /// <summary>
    /// مدل اطلاعات پایه دریافتی از appSetting
    /// </summary>
    public class FajrLogBaseDTO
    {
        #region مشخصات نرم افزار ارسال کننده لاگ
        /// <summary>
        /// نام سامانه 
        /// </summary>
        public string appName { get; set; }
        /// <summary>
        /// نسخه سامانه
        /// </summary>
        public string appVersion { get; set; }
        /// <summary>
        /// کد سامانه به ازای هر سامانه منحصر به فرد بوده و توسط نماینده ساحفا تعیین و اعلام می شود
        /// </summary>
        public long? appId { get; set; }
        /// <summary>
        /// تولید کننده سامانه
        /// </summary>
        public string appVendor { get; set; }
        /// <summary>
        /// سرور سامانه IP
        /// </summary>
        public string appServerIP { get; set; }
        /// <summary>
        /// نام سرور سامانه
        /// </summary>
        public string appServerHostName { get; set; }
        /// <summary>
        /// شماره پورت
        /// </summary>
        public string appPortNum { get; set; }
        public string appDBIP { get; set; }
        public string appDBName { get; set; }
        #endregion



        #region مشخصات یگان
        /// <summary>
        /// نام نیرو
        /// </summary>
        public string forceName { get; set; }

        /// <summary>
        /// به سازمان ارائه خواهد شد 
        /// </summary>
        public long? forceUniqueId { get; set; }

        /// <summary>
        /// نام سازمان
        /// </summary>
        public string orgName { get; set; }

        /// <summary>
        /// به سازمان ارائه خواهد شد 
        /// </summary>
        public long? orgUniqueId { get; set; }

        /// <summary>
        /// نام صنعت
        /// </summary>
        public string depName { get; set; }
        public long? depUniqueId { get; set; }

        /// <summary>
        /// نام بخش
        /// </summary>
        public string secName { get; set; }
        public long? secUniqueId { get; set; }

        /// <summary>
        /// نام واحد
        /// </summary>
        public string partName { get; set; }
        public long? partUniqueId { get; set; }

        /// <summary>
        /// نام منطقه
        /// </summary>
        public string zoneName { get; set; }
        public long? zoneId { get; set; }

        /// <summary>
        /// نام شهر
        /// </summary>
        public string cityName { get; set; }
        public long? cityId { get; set; }
        #endregion
    }
}
