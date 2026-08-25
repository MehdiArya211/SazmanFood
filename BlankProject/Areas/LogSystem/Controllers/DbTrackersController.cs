using BLL.Interface;
using BLL.LogSystem.UserChangeLogEvent;
using Domain.Entities;
using DTO.Entities.LogSystem.UserChangeLogEvent;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.RedisService;

namespace LearningSkill.Areas.LogSystem.Controllers
{
    [Area("LogSystem")]
    [UserAuthorize(Area: "LogSystem", Controller: "DbTrackers", Action: "index")]
    public class DbTrackersController : Controller
    {
        private readonly IRedisManager Redis;
        private readonly IDataTableManager dataTableManager;
        private readonly IUserChangeLogEventManager _userChangeLogEventManager;

        public DbTrackersController(IRedisManager _Redis, IUserChangeLogEventManager userChangeLogEventManager, IDataTableManager _dataTableManager)
        {
            Redis = _Redis;
            dataTableManager = _dataTableManager;
            _userChangeLogEventManager = userChangeLogEventManager;
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
        public ActionResult GetList(UserChagneLogEventFilter filters)
        {
            var SearchModel = dataTableManager.GetSearchModel();
            var model = _userChangeLogEventManager.GetDataTableDTO(SearchModel, filters);
            return Json(model);
        }

        #endregion

        #region مشاهده جزئیات

        public IActionResult LoadDetailForm(long id)
        {
            var model = _userChangeLogEventManager.GetDetailDTO(id);
            return View(model);
        }

        #endregion
    }
}
