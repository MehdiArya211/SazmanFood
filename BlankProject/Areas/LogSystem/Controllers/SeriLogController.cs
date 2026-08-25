using BLL;
using BLL.Interface;
using DTO;
using DTO.User;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearningSkill.Areas.LogSystem.Controllers
{
    [Area("LogSystem")]
    [UserAuthorize(Area: "LogSystem", Controller: "SeriLog", Action: "index")]
    public class SeriLogController : Controller
    {
        private readonly IDataTableManager dataTableManager;
        private readonly ISeriLogManager seriLogManager;
        public SeriLogController(IDataTableManager _dataTableManager, ISeriLogManager _seriLogManager)
        {
            dataTableManager = _dataTableManager;
            seriLogManager = _seriLogManager;
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
        public ActionResult GetList(SeriLogFilter filters)
        {
            var SearchModel = dataTableManager.GetSearchModel();
            var model = seriLogManager.GetDataTableDTO(SearchModel, filters);
            return Json(model);
        }
        #endregion
    }
}
