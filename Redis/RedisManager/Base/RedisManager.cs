using Microsoft.AspNetCore.Http;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Services.RedisService
{
    /// <summary>
    /// مدیریت ردیس
    /// </summary>
    public class RedisManager : IRedisManager
    {
        //private readonly IConnectionMultiplexer Multiplexer;
        private readonly IRedisClient RedisClient;
        private readonly IHttpContextAccessor HttpContextAccessor;

        public RedisManager(/*IConnectionMultiplexer _Multiplexer,*/ IRedisClient _RedisClient, IHttpContextAccessor _HttpContextAccessor)
        {
            //Multiplexer = _Multiplexer;
            RedisClient = _RedisClient;
            HttpContextAccessor = _HttpContextAccessor;
        }


        /// <summary>
        /// دیتابیس ردیس
        /// </summary>
        public IRedisDatabase db => RedisClient.GetDb(0);



        /// <summary>
        /// کانتکس اکسسور
        /// </summary>
        public IHttpContextAccessor ContextAccessor => HttpContextAccessor;



       

    }
}
