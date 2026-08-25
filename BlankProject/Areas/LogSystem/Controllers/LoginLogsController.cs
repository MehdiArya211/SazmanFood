using BLL;
using BLL.Interface;
using Domain.Enums;
using DTO.UserLog;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.RedisService;

namespace Food.Areas.LogSystem.Controllers
{
    [Area("LogSystem")]
    [UserAuthorize(Area: "LogSystem", Controller: "LoginLogs", Action: "index")]
    public class LoginLogsController : Controller
    {
        private readonly IUserLogManager logManager;
        private readonly IDataTableManager dataTableManager;
        private readonly IRedisManager Redis;
        public LoginLogsController(IRedisManager _Redis, IUserLogManager _logManager, IDataTableManager _dataTableManager)
        {
            logManager = _logManager;
            dataTableManager = _dataTableManager;
            Redis = _Redis;
        }


        #region نمایش همه
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// لیست داده مورد نیاز برای دیتاتیبل
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetList(UserLoginLogFilter filters)
        {
            var SearchModel = dataTableManager.GetSearchModel();
            var model = logManager.GetLoginDataTableDTO(SearchModel, filters);
            return Json(model);
        }

        #endregion


        #region خروجی اکسل از لیست
        public IActionResult DownloadExcel(UserLoginLogFilter filters)
        {
            var model = logManager.GetReportExcel(filters);
            if (model == null)
                model = new List<UserLoginLogDataTableDTO>();

            var ReportGenerator = new DynamicReportGenerator<UserLoginLogDataTableDTO>();

            var notAllowed = new List<string> { "issuccess", "createdate" };
            var props = ReportGenerator.GetProperties()
                                       .Select(x => x.TitleEn)
                                       .Where(x => !notAllowed.Contains(x.ToLower()))
                                       .ToList();

            var wb = ReportGenerator.GenerateReportWorkBook("لاگ ورود و خروج", model, props);

            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                //Return xlsx Excel File  
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.DownloadExcel, MenuType.LoginLog, true).Result;
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "loginLogs.xlsx");
            }
        }
        #endregion


    }
}
