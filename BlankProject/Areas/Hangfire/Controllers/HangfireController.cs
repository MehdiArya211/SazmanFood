using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace LearningSkill.Areas.Hangfire.Controllers
{
    public class HangfireController : Controller
    {
        private readonly IRecurringJobManager _recurringJobManager;
        public HangfireController(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        [HttpPost]
        [Route("backup")]
        public IActionResult BackUp(string userName)
        {
            _recurringJobManager.AddOrUpdate("test", () => BackUpDataBase(), Cron.Minutely);
            return Ok();
        }

        public void BackUpDataBase()
        {
            Console.WriteLine("Recurring!");
        }
    }
}
