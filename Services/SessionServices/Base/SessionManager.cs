using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.SessionServices
{
    /// <summary>
    /// گرفتن سشن فعلی
    /// </summary>
    public class SessionManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public ISession Session;

        public SessionManager(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }
    }
}
