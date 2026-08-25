using Utilities.Extentions;
using FajrLog.DTO;
using FajrLog.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FajrLog.Domain
{
    /// <summary>
    /// مدلی که باید برای لاگ فجر ارسال شود
    /// </summary>
    public class FajrLogEntity
    {
        #region چه زمانی
        /// <summary>
        /// زمان ثبت رویداد در فجر
        /// </summary>
        public string timeStamp { get; set; }

        /// <summary>
        /// زمان رخ دادن رویداد سمت کلاینت. در پروژه وب همون timeRegister
        /// </summary>
        public long timeOccurrence { get; set; }

        /// <summary>
        /// زمان ثبت رویداد سمت سرور سامانه
        /// </summary>
        public long timeRegister { get; set; }

        /// <summary>
        /// بازه زمانی 
        /// </summary>
        public string timeDuration { get; set; }
        public string timeSend { get; set; }
        public string timeFrom { get; set; }
        public string timeTo { get; set; }
        #endregion


        #region چه رویدادی
        /// <summary>
        /// حیطه رویداد اتفاق افتاده
        /// </summary>
        public string actionType { get; set; }

        /// <summary>
        /// نوع رویداد اتفاق افتاده
        /// </summary>
        public string actionSubType { get; set; }

        /// <summary>
        /// توضیحاتی در خصوص رویداد (در صورت وجود در سامانه مثلا دلیل غیر فعال شدن کاربر)
        /// </summary>
        public string actionDescription { get; set; }

        /// <summary>
        /// شناسه هر رویداد ( برای رویدادهای فاقد شناسه به صورت توافقی بین حفا و تولید کننده تعیین می گردد ) مرتبط با زیر رویداد است (actionSubType )
        /// </summary>
        public long? actionId { get; set; }

        /// <summary>
        /// وضعیت موفقیت عملیات
        /// <para>
        /// Success/UnSuccess/Fail/Allow/Deny
        /// </para>
        /// </summary>
        public string actionFlag { get; set; }

        /// <summary>
        /// میزان حساسیت عملیات
        /// <para>
        /// Info/Low/High/Critical
        /// </para>
        /// </summary>
        public string actionSensitivity { get; set; }

        /// <summary>
        /// کد هر لاگ (عددی غیر تکراری و منحصر به فرد)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string logId { get; set; }

        /// <summary>
        /// شماره ردیف لاگ
        /// </summary>
        public long? logNum { get; set; }
        #endregion


        #region چه کسی
        public string userName { get; set; }
        public string consoleUser { get; set; }
        public string nationalId { get; set; }
        public long? uniqueId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public long? employeeNum { get; set; }

        /// <summary>
        /// نوع عضویت (رسمی، قراردادی، شرکتی،مهمان و ... )
        /// </summary>
        public string membership { get; set; }

        /// <summary>
        /// سطح دسترسی(محرمانه، خیلی محرمانه، عادی و ... )
        /// </summary>
        public string accessLevel { get; set; }

        /// <summary>
        /// ملیت
        /// </summary>
        public string nationality { get; set; }

        /// <summary>
        /// جنسیت
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// شماره تلفن
        /// </summary>
        public string phoneNum { get; set; }
        public string groupName { get; set; }
        public string carNum { get; set; }

        /// <summary>
        /// تصویر کاربر
        /// </summary>
        public string pic { get; set; }
        public string comment { get; set; }
        #endregion


        #region چه مکانی
        /// <summary>
        /// آدرس URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// نام فرم ایجاد رویداد
        /// </summary>
        public string formName { get; set; }

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

        /// <summary>
        /// نام گیت ورود خروج در سامانه های کنترل تردد
        /// </summary>
        public string gateName { get; set; }

        /// <summary>
        /// نام زیر سامانه ایجاد رویداد
        /// </summary>
        public string subSystemName { get; set; }
        #endregion


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


        #region کلاینت
        public string clientName { get; set; }
        public long? clientUniqueId { get; set; }
        public string MACAddress { get; set; }
        public string HDDSerial { get; set; }

        /// <summary>
        /// IP (این فیلد همیشه شامل مشخصات عامل رویداد است) اولویت تکمیل
        /// </summary>
        public string IP { get; set; }
        public string OS { get; set; }
        public string userAgent { get; set; }
        #endregion


        #region سخت افزار
        public string deviceName { get; set; }
        public string deviceType { get; set; }
        public long? deviceUniqueId { get; set; }
        public string deviceComment { get; set; }
        public string deviceStatus { get; set; }
        public string deviceBusType { get; set; }
        #endregion


        #region انواع موجودیت
        /// <summary>
        /// نوع موجودیتی که عملیات روی آن اتفاق افتاده است.
        /// <para>
        /// contract قرارداد,product,device,file,app,  متن msg,personal,malware
        /// </para>
        /// </summary>
        public string targetType { get; set; }

        /// <summary>
        /// "دیوایس(هارد، CPU,پرینتر،کارت شبکه
        /// فایل file(متنی، تصویری، صوتی، سیستمی،doc, xlsx, pdf, ppt, dll, exe,...
        /// نرم افزار app(فایروال،نرم افزار کاربردی، نرم افزار امنیتی، سیستم عامل، …
        /// Admin, SuperUser, User ) personal یا دسته بندی کلی از نوع همکاری شخص(پرسنل، مهمان، شرکت همکار،سرباز و  ...)
        /// contract قرارداد : نوع قرارداد
        /// contractor پیمانکار شرکت همکار
        /// trojan,worm : malware ,…."
        /// </summary>
        public string targetSubType { get; set; }

        /// <summary>
        /// کد یکتای موجودیت (غیر از موارد خاص این فیلد همیشه شامل مشخصات مفعول رویداد است) اولویت در تکمیل
        /// </summary>
        public long? targetUniqueId { get; set; }

        /// <summary>
        /// اولویت یک، خصوصیات موجودیت
        /// </summary>
        public string targetSpec { get; set; }
        public string targetContent { get; set; }

        /// <summary>
        /// اولویت سه، توضیحات موجودیت در این فیلد ثبت می شود (بهتر است شامل رشته های بزرگ نباشد اولویت تکمیل با targetDescription,targetSpec)
        /// </summary>
        public string targetComment { get; set; }

        /// <summary>
        /// در رویدادهایی که تارگت یک دیوایس یا فایل و ... است
        /// </summary>
        public string targetName { get; set; }

        /// <summary>
        /// .(غیر از موارد خاص این فیلد همیشه شامل مشخصات مفعول رویداد است) اولویت در تکمیل IP
        /// </summary>
        public string targetIP { get; set; }
        public string targetUserAgent { get; set; }

        /// <summary>
        /// در رویدادهای خاص دارای مقدار است (رویدادهایی که هدف فقط کاربر نیست ودو عامل وجود داشته باشد)
        /// </summary>
        public string targetUserName { get; set; }
        /// <summary>
        /// در رویدادهای خاص دارای مقدار است (رویدادهایی که هدف فقط کاربر نیست ودو عامل وجود داشته باشد)
        /// </summary>
        public string targetUserIP { get; set; }
        //public long? targetUniqueId { get; set; }
        public string targetFirstName { get; set; }
        public string targetLastName { get; set; }
        public string targetNatioanalId { get; set; }

        /// <summary>
        /// کد پرسنلی
        /// </summary>
        public long? targetEmployeeNum { get; set; }

        /// <summary>
        /// نوع عضویت (رسمی، قراردادی، شرکتی،مهمان و ... )
        /// </summary>
        public string targetMembership { get; set; }

        /// <summary>
        /// سطح دسترسی (فاقد،محرمانه،خیلی محرمانه و...)
        /// </summary>
        public string targetAccessLevel { get; set; }
        public string targetNationality { get; set; }
        public string targetGender { get; set; }

        /// <summary>
        /// سطح حساسیست (حساس، مهم و...)
        /// </summary>
        public string targetSensitivity { get; set; }
        public string targetPhoneNum { get; set; }
        public string targetCarNum { get; set; }
        public string targetPic { get; set; }
        public string targetMACAddress { get; set; }
        public string targetHDDSerial { get; set; }
        public string targetConsolName { get; set; }

        /// <summary>
        /// به طور مثال دلیل حضور مهمان در صنعت
        /// </summary>
        public string targetSubject { get; set; }
        public string targetGroupName { get; set; }
        public long? targetId { get; set; }
        public string targetHash { get; set; }
        public string targetPath { get; set; }
        public string targetSize { get; set; }
        public string targetBusType { get; set; }
        public string targetCreateTime { get; set; }
        public string targetModificationTime { get; set; }
        public string targetVendor { get; set; }
        public string targetVersion { get; set; }

        /// <summary>
        /// مقیاس، اندازه، واحد،کیلو،عدد، دستگاه، امتیاز، نمره
        /// </summary>
        public string targetMessure { get; set; }

        /// <summary>
        /// مقدار
        /// </summary>
        public string targetAmount { get; set; }
        #endregion








        #region CONSTRUCTOR
        public FajrLogEntity()
        {

        }


        public FajrLogEntity(IHttpContextAccessor _contextAccessor, FajrLogBaseDTO baseInfo, FajrActionType actionType, FajrActionFlag flag, FajrActionSensitivity sensitivity,
            string username, long? userId = null, string FullName = null, string description = null, long? targetId = null)
            : this(actionType, baseInfo)
        {
            userName= username;
            uniqueId = userId;
            firstName = FullName;
            actionFlag = flag.ToString();
            actionSensitivity = sensitivity.ToString();
            actionDescription = description;
            targetUniqueId = targetId;

            var time = DateTimeOffset.Now.ToUnixTimeSeconds();
            timeOccurrence = time;
            timeRegister = time;

            IP = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var request = _contextAccessor.HttpContext.Request;
            URL = $"{request.Scheme}//{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
        }


        #region مقدار دهی اطلاعات بر اساس اینام نوع اکشن
        public FajrLogEntity(FajrActionType _actionType, FajrLogBaseDTO baseInfo) : this(baseInfo)
        {
            var info = _actionType.GetEnumAttribute<FajrActionTypeInfoAttribute>();
            actionId = info.ActionId;
            actionType = info.ActionType;
            actionSubType = info.ActionSubType;
        }
        #endregion


        #region مقدار دهی اطلاعات پایه
        /// <summary>
        /// مقدار دهی اطلاعات پایه
        /// </summary>
        /// <param name="baseInfo"></param>
        public FajrLogEntity(FajrLogBaseDTO baseInfo)
        {
            try
            {
                #region اطلاعات پایه از appseting
                #region مشخصات نرم افزار ارسال کننده لاگ
                appName = baseInfo.appName;
                appVersion = baseInfo.appVersion;
                appId = baseInfo.appId;
                appVendor = baseInfo.appVendor;
                appServerIP = baseInfo.appServerIP;
                appServerHostName = baseInfo.appServerHostName;
                appPortNum = baseInfo.appPortNum;
                appDBIP = baseInfo.appDBIP;
                appDBName = baseInfo.appDBName;
                #endregion



                #region مشخصات یگان
                forceName = baseInfo.forceName;
                forceUniqueId = baseInfo.forceUniqueId;
                orgName = baseInfo.orgName;
                orgUniqueId = baseInfo.orgUniqueId;
                depName = baseInfo.depName;
                depUniqueId = baseInfo.depUniqueId;
                secName = baseInfo.secName;
                secUniqueId = baseInfo.secUniqueId;
                partName = baseInfo.partName;
                partUniqueId = baseInfo.partUniqueId;
                zoneName = baseInfo.zoneName;
                zoneId = baseInfo.zoneId;
                cityName = baseInfo.cityName;
                cityId = baseInfo.cityId;
                #endregion
                #endregion
            }
            catch
            {

            }
        } 
        #endregion
        #endregion




    }
}