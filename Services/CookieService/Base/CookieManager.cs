using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.CookieServices
{
    /// <summary>
    /// گرفتن کوکی فعلی
    /// </summary>
    public class CookieManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public HttpContext HttpContext;
        public HttpResponse Response;
        public HttpRequest Request;
        public IRequestCookieCollection RequestCookies;
        public IResponseCookies ResponseCookies;

        public CookieManager(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
            HttpContext = httpContextAccessor.HttpContext;
            Request = HttpContext.Request;
            Response = HttpContext.Response;
            RequestCookies = Request.Cookies;
            ResponseCookies = Response.Cookies;
        }
    }
}
