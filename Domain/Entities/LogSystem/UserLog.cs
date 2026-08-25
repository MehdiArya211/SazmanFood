using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Utilities.Extentions;


namespace Domain.Entities
{
    public class UserLog : EntityBase
    {
        [Display(Name = "آیپی کاربر")]
        public string UserIp { get; set; }


        [Display(Name = "شناسه کاربر")]
        public long? UserId { get; set; }


        [Display(Name = "نام کاربر")]
        public string FullName { get; set; }


        [Display(Name = "آدرس ریکوئست")]
        public string Url { get; set; }


        [Display(Name = "نوع عملیات")]
        public ActionType ActionType { get; set; }


        [Display(Name = "نام عملیات")]
        public string ActionName { get; set; }


        [Display(Name = "نوع منو")]
        public MenuType MenuType { get; set; }


        [Display(Name = "نام منو")]
        public string MenuName { get; set; }


        [Display(Name = "توضیحات")]
        public string Description { get; set; }


        [Display(Name = "عملیات موفق بوده؟")]
        public bool IsSuccess { get; set; }


        [Display(Name = "آیدی موجودیت")]
        public long? TargetId { get; set; }



        #region کلاس های سازنده

        /// <summary>
        /// سازنده لاگ عمومی
        /// </summary>
        public UserLog(IHttpContextAccessor _contextAccessor, ActionType actionType, MenuType menuType,
             string fullName = null, long? userId = null, bool isSuccess = true, string description = null, long? targetId = null, bool IsApi = false) : base()
        {
            IsSuccess = isSuccess;
            UserIp = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            ActionType = actionType;
            ActionName = actionType.GetEnumDescription();
            MenuType = menuType;
            MenuName = menuType.GetEnumDescription();
            FullName = fullName;
            UserId = userId;
            TargetId = targetId;
            if (IsApi)
                Description = "(Api) " + description;
            else
                Description = description;

            var request = _contextAccessor.HttpContext.Request;
            Url = $"{request.Scheme}//{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
        }



        /// <summary>
        /// سازنده برای عملیات لاگین
        /// </summary>
        /// <param name="_contextAccessor"></param>
        /// <param name="fullName">نام کاربری که سعی در لاگین داشت</param>
        /// <param name="isSuccess">وضعیت عملیات</param>
        public UserLog(IHttpContextAccessor _contextAccessor, string fullName, long? userId = null, bool isSuccess = true, string description = null, bool IsLogOut = false, bool IsApi = false) : base()
        {
            IsSuccess = isSuccess;
            UserIp = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            ActionType = IsLogOut ? ActionType.LogOff : ActionType.Login;
            ActionName = (IsLogOut ? ActionType.LogOff : ActionType.Login).GetEnumDescription();
            MenuType = MenuType.Login;
            MenuName = MenuType.Login.GetEnumDescription();
            FullName = fullName;
            UserId = userId;
            if (IsApi)
                Description = "(Api) " + description;
            else
                Description = description;

            var request = _contextAccessor.HttpContext.Request;
            Url = $"{request.Scheme}//{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
        }





        public UserLog() : base()
        {

        }
        #endregion
    }
}
