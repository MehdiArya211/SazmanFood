using BLL.Interface;
using Domain.Enums;
using FajrLog.Enum;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Services.RedisService;
using Utilities.Extentions;

namespace Food.Areas.Shared.Controllers
{
    /// <summary>
    /// پارامتر تنظیمات
    /// </summary>
    [Area("Shared")]
    [UserAuthorize(Area: "shared", Controller: "constants", Action: "index")]
    public class ConstantsController : Controller
    {
        private readonly IConstantManager constantManager;
        private readonly IDataTableManager dataTableManager;
        private readonly IRedisManager Redis;
        public ConstantsController(IRedisManager _Redis, IConstantManager _constantManager, IDataTableManager _dataTableManager)
        {
            constantManager = _constantManager;
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
        public ActionResult GetList()
        {
            var SearchModel = dataTableManager.GetSearchModel();
            var model = constantManager.GetDataTableDTO(SearchModel);
            return Json(model);
        }

        #endregion


        #region ویرایش
        /// <summary>
        /// لود کردن فرم ویرایش در مدال
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadEditForm(long id)
        {
            var model = constantManager.GetById(id);
            return PartialView("_Edit", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long Id, string Value, ConstantType Type)
        {
            if (ModelState.IsValid)
            {
                var res = constantManager.Update(Id, Value);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Update, MenuType.Constants, res.Status, $"آیدی : {Id} | عنوان : {Type.GetEnumDescription()} | توضیحات :{res.Message}", Id, FajrActionType.editSetting).Result;
                return Json(res);
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _ = Redis.db.SetLog(Redis.ContextAccessor, ActionType.Update, MenuType.Constants, false, $"آیدی : {Id} | عنوان : {Type.GetEnumDescription()} | توضیحات :{string.Join("/", errors)}", Id, FajrActionType.editSetting).Result;
                return Json(new
                {
                    Status = false,
                    Message = string.Join("</br>", errors)
                });
            }
        }

        #endregion


    }
}
