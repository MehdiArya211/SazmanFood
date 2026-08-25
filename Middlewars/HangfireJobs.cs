using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Hangfire;
using System.Threading.Tasks;


namespace HangfireJobs.Middlewars
{
    public class hangfireNezaja
    {
        private readonly RequestDelegate _next;
        private readonly IRecurringJobManager _recurringJobManager;
        public hangfireNezaja(RequestDelegate next, IRecurringJobManager recurringJobManager)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _recurringJobManager = recurringJobManager;
        }

        public async Task Invoke(HttpContext context)
        {
             _recurringJobManager.AddOrUpdate("test", () => BackUpDataBase(), Cron.Minutely);
            await _next(context).ConfigureAwait(false);
        }
        public void BackUpDataBase()
        {
            var d = "df";
        }
    }

    public static class HangfireExtension
    {
        public static IApplicationBuilder MapHangfireJobs(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<hangfireNezaja>();
        }
    }

  

}
