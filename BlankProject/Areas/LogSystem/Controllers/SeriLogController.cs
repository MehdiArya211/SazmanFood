using BLL;
using BLL.Interface;
using DTO;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.SessionServices;

namespace Food.Areas.LogSystem.Controllers
{
    [Area("LogSystem")]
    public class SeriLogController : Controller
    {
        private readonly IDataTableManager dataTableManager;
        private readonly ISeriLogManager seriLogManager;

        public SeriLogController(IDataTableManager _dataTableManager, ISeriLogManager _seriLogManager)
        {
            dataTableManager = _dataTableManager;
            seriLogManager = _seriLogManager;
        }

        [UserAuthorize(Area: "LogSystem", Controller: "SeriLog", Action: "index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [UserAuthorize(Area: "LogSystem", Controller: "SeriLog", Action: "index")]
        public ActionResult GetList(SeriLogFilter filters)
        {
            var searchModel = dataTableManager.GetSearchModel();
            var model = seriLogManager.GetDataTableDTO(searchModel, filters);
            return Json(model);
        }

        /// <summary>
        /// صفحه گزارش خطاهای ثبت‌شده در منوی بانک اطلاعاتی.
        /// از نمای لاگ موجود استفاده می‌شود تا مسیر ثبت‌شده در منو فعال باشد.
        /// </summary>
        [HttpGet]
        [UserAuthorize(IsPublic: true)]
        public IActionResult Diagnostics()
        {
            var currentUser = HttpContext.Session.GetUser();
            if (currentUser == null || !currentUser.HasMenu("logsystem", "serilog", "diagnostics"))
                return Forbid();

            return View("Index");
        }
    }
}
