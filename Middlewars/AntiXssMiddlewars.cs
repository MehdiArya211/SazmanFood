using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Ganss.Xss;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AntiXssMiddleware.Middleware
{
    public class AntiXssMiddleware
    {
        private readonly RequestDelegate _next;
        private ErrorResponse _error;
        private readonly int _statusCode = (int)HttpStatusCode.BadRequest;
        private readonly List<string> AllowedUrls = new List<string>
        { "shared/extrapages", "blogsystem/posts", "blogsystem/faqs", "shoppingsystem/products" };


        public AntiXssMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            // Check XSS in URL
            var url = context.Request.Path.Value;
            if (!string.IsNullOrWhiteSpace(context.Request.Path.Value))
            {
                if (CrossSiteScriptingValidation.IsDangerousString(url, out _))
                {
                    await RespondWithAnError(context).ConfigureAwait(false);
                    return;
                }
            }

            // Check XSS in query string
            if (!string.IsNullOrWhiteSpace(context.Request.QueryString.Value))
            {
                var queryString = WebUtility.UrlDecode(context.Request.QueryString.Value);

                if (CrossSiteScriptingValidation.IsDangerousString(queryString, out _))
                {
                    await RespondWithAnError(context).ConfigureAwait(false);
                    return;
                }
            }

            // Check XSS in request content
            #region آدرس درخواست جزو ادرس های مجاز است؟
            url = url.ToLower();
            bool isAllowed = false; // AllowedUrls.Any(x => url.Contains(x));
            #endregion

            var originalBody = context.Request.Body;
            try
            {
                #region بررسی اطلاعات ارسال شده در قالب فرم
                if (context.Request.HasFormContentType)
                {
                    var form = await context.Request.ReadFormAsync();

                    #region بررسی ایتم های فرم و حذف تگ های خطرناک
                    var dic = new Dictionary<string, StringValues>();
                    bool hasDangerouseTag = false;
                    foreach (var item in form)
                    {
                        var val = GetFormCollectionSanitizeValue(item.Value, !isAllowed);
                        if (!isAllowed && val != item.Value)
                        {
                            hasDangerouseTag = true;
                            break;
                        }
                        dic.Add(item.Key, val);
                    }
                    if (hasDangerouseTag)
                    {
                        await RespondWithAnError(context).ConfigureAwait(false);
                        return;
                    }
                    var formCol = new FormCollection(dic, form.Files);
                    context.Request.Form = formCol;
                    #endregion
                }
                #endregion


                #region بررسی اطلاعات ارسال شده در بادی
                else
                {
                    var body = await ReadRequestBody(context);
                    #region بررسی بادی و دادن خطای مرتبط
                    if (!string.IsNullOrEmpty(body) && CrossSiteScriptingValidation.IsDangerousString(body, out _))
                    {
                        await RespondWithAnError(context).ConfigureAwait(false);
                        return;
                    }
                    #endregion
                } 
                #endregion

                await _next(context).ConfigureAwait(false);
            }
            finally
            {
                context.Request.Body = originalBody;
            }
        }



        /// <summary>
        /// تبدیل مقادیر درون دیتای فرم به حالت بدون تگ های خطرناک
        /// به خصوص برای زمانی که یک کلید چندین مقدار دارد
        /// </summary>
        /// <param name="Value">مقدار متناظر با هر کلید</param>
        /// <param name="RemoveAllHTML">حذف تمام تگ های html یا فقط تگ های خطرناک</param>
        /// <returns></returns>
        private StringValues GetFormCollectionSanitizeValue(StringValues Value, bool RemoveAllHTML)
        {
            List<string> OutValue = new List<string>();
            HtmlSanitizer sanitizer = null;
            if (RemoveAllHTML)
                sanitizer = new HtmlSanitizer(new HtmlSanitizerOptions
                {
                    AllowedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                });
            else
                sanitizer = new HtmlSanitizer();
            if (Value.Count() > 0)
            {
                foreach (var v in Value)
                {
                    var html = HttpUtility.HtmlDecode(v);
                    var val = sanitizer.Sanitize(html);
                    val = val.Replace("\n", "\r\n");
                    OutValue.Add(HttpUtility.HtmlDecode(val));
                }
            }
            else
            {
                var val = sanitizer.Sanitize(Value);
                OutValue.Add(HttpUtility.HtmlDecode(val));
            }

            StringValues model = new StringValues(OutValue.ToArray());
            return model;
        }






        private static async Task<string> ReadRequestBody(HttpContext context)
        {
            var buffer = new MemoryStream();
            await context.Request.Body.CopyToAsync(buffer);
            context.Request.Body = buffer;
            buffer.Position = 0;

            var encoding = Encoding.UTF8;

            var requestContent = await new StreamReader(buffer, encoding).ReadToEndAsync();
            context.Request.Body.Position = 0;

            return requestContent;
        }

        private async Task RespondWithAnError(HttpContext context)
        {
            //context.Response.Clear();
            context.Response.Headers.AddHeaders();
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = 406; // Not Acceptable

            //if (_error == null)
            //{
            //    _error = new ErrorResponse
            //    {
            //        Status = false,
            //        Message = "استفاده از تگ های HTML و اسکریپت در درخواست مجاز نمی باشد!",
            //        ErrorCode = 400
            //    };
            //}
            var obj = new
            {
                status = false,
                message = "استفاده از تگ های HTML و اسکریپت در درخواست مجاز نمی باشد!"
            };

            await context.Response.WriteAsync(obj.ToJSON());
        }
    }

    public static class AntiXssMiddlewareExtension
    {
        public static IApplicationBuilder UseAntiXssMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiXssMiddleware>();
        }
    }


    /// <summary>
    /// Imported from System.Web.CrossSiteScriptingValidation Class
    /// </summary>
    public static class CrossSiteScriptingValidation
    {
        private static readonly char[] StartingChars = { '<', '&' };

        #region Public methods

        public static bool IsDangerousString(string s, out int matchIndex)
        {
            //bool inComment = false;
            matchIndex = 0;

            for (var i = 0; ;)
            {

                // Look for the start of one of our patterns 
                var n = s.IndexOfAny(StartingChars, i);

                // If not found, the string is safe
                if (n < 0) return false;

                // If it's the last char, it's safe 
                if (n == s.Length - 1) return false;

                matchIndex = n;

                switch (s[n])
                {
                    case '<':
                        // If the < is followed by a letter or '!', it's unsafe (looks like a tag or HTML comment)
                        if (IsAtoZ(s[n + 1]) || s[n + 1] == '!' || s[n + 1] == '/' || s[n + 1] == '?') return true;
                        break;
                    case '&':
                        // If the & is followed by a #, it's unsafe (e.g. S) 
                        if (s[n + 1] == '#') return true;
                        break;

                }

                // Continue searching
                i = n + 1;
            }
        }

        #endregion

        #region Private methods

        private static bool IsAtoZ(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        #endregion

        public static void AddHeaders(this IHeaderDictionary headers)
        {
            if (headers["P3P"].IsNullOrEmpty())
            {
                headers.Add("P3P", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
        public static string ToJSON(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }

    public class ErrorResponse
    {
        public bool Status { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }

}