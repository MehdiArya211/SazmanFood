using Domain.Entities;
using DTO.Base;
using DTO.Entities.MaxaRabbitMQ;
using DTO.Menu;
using DTO.User;
using Infrastructure.Data;

namespace BLL.Interface
{
    /// <summary>
    /// عملیات احراز هویت و لاگین
    /// </summary>
    public interface IAuthManager : IManager<User, ApplicationContext>
    {

        /// <summary>
        /// لاگین کاربر
        /// </summary>
        /// <param name="Username">نام کاربری</param>
        /// <param name="Password">کلمه عبور</param>
        /// <returns></returns>
        BaseResult Login(string Username = null, string Password = null, int? UserId = null);

        /// <summary>
        /// لاکین کاربر با دستگاه تشخیص صورت
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserSessionDTO LoginWithFace(AccessLogMonitoringEvent deviceEvent);
        public UserSessionDTO LoginWithFaceZP(long userId);

        BaseResult LoginAuth( string UserId );


        /// <summary>
        /// لاگ اوت کاربر
        /// </summary>
        void Logout();


        /// <summary>
        ///  داده خامی که از منوهای کاربر دریافت می شود،
        ///  به صورت ساختار درختی واقعی در می آید
        /// </summary>
        /// <param name="data">داده خام خارج از ساختار درختی</param>
        /// <param name="parentId">آیدی والد</param>
        /// <returns></returns>
        List<MenuSessionDTO> ReshapeMenuData(IEnumerable<MenuSessionDTO> data, long? parentId = null);


    }
}
