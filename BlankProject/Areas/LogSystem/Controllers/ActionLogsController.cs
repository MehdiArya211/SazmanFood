using BLL;
using BLL.Interface;
using Domain.Enums;
using DTO.UserLog;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.RedisService;
using Utilities.Extentions;

namespace LearningSkill.Areas.Shared.Controllers
{
    [Area("LogSystem")]
    [UserAuthorize(Area: "LogSystem", Controller: "ActionLogs", Action: "index")]
    public class ActionLogsController : Controller
    {
        private readonly IUserLogManager logManager;
        private readonly IDataTableManager dataTableManager;
        private readonly IRedisManager Redis;
        public ActionLogsController(IRedisManager _Redis, IUserLogManager _logManager, IDataTableManager _dataTableManager)
        {
            logManager = _logManager;
            dataTableManager = _dataTableManager;
            Redis = _Redis;
        }


        #region نمایش همه
        public IActionResult Index()
        {
            ViewData["ActionType"] = new SelectList(EnumExtensions.ToEnumViewModel<ActionType>(), "Id", "Title");
            ViewData["MenuType"] = new SelectList(EnumExtensions.ToEnumViewModel<MenuType>(), "Id", "Title");
            return View();
        }


        /// <summary>
        /// لیست داده مورد نیاز برای دیتاتیبل
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetList(UserActionLogFilter filters)
        {
            var SearchModel = dataTableManager.GetSearchModel();
            var model = logManager.GetDataTableDTO(SearchModel, filters);
            return Json(model);
        }

        #endregion


        #region خروجی اکسل از لیست
        public IActionResult DownloadExcel(UserActionLogFilter filters)
        {
            var model = logManager.GetReportExcel(filters);
            if (model == null)
                model = new List<UserActionLogDataTableDTO>();

            var ReportGenerator = new DynamicReportGenerator<UserActionLogDataTableDTO>();

            var notAllowed = new List<string> { "issuccess", "createdate" };
            var props = ReportGenerator.GetProperties()
                                       .Select(x => x.TitleEn)
                                       .Where(x => !notAllowed.Contains(x.ToLower()))
                                       .ToList();

            var wb = ReportGenerator.GenerateReportWorkBook("لاگ فعالیت ها", model, props);

            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                //Return xlsx Excel File  
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.DownloadExcel, MenuType.ActionLog, true).Result;
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "actionLogs.xlsx");
            }
        }
        #endregion


    }
}
