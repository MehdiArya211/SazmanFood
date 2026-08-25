using BLL.FoodManag.KitchenBL;
using BLL.Interface;
using DTO.Entities.Kitchen;
using Filters;
using ITOWebApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.RedisService;

namespace Food.Areas.FoodMang.Controllers
{
    /// <summary>
    /// تعریف آشپزخانه
    /// </summary>
    [Area("FoodMang")]
    [UserAuthorize(Area: "FoodMang", Controller: "Kitchens", Action: "index")]
    public class KitchensController : Controller
    {
        private readonly IDataTableManager dataTableManager;
        private readonly IKitchensManager kitchensManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession Session;
        private readonly IRedisManager Redis;
        private readonly ApiTokenCacheClient apiTokenClient;
        private readonly IWebApiManager webApiManager;

        private string access_token = string.Empty;

        public KitchensController(
            IRedisManager _Redis,
            IHttpContextAccessor _httpContextAccessor,
            IDataTableManager _dataTableManager,
            IKitchensManager _kitchensManager,
            ApiTokenCacheClient _apiTokenClient,
            IWebApiManager _webApiManager)
        {
            Redis = _Redis;
            httpContextAccessor = _httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;

            dataTableManager = _dataTableManager;
            kitchensManager = _kitchensManager;
            apiTokenClient = _apiTokenClient;
            webApiManager = _webApiManager;


        }

        #region نمایش

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetList(KitchenFilterDTO filters)
        {
            var searchModel = dataTableManager.GetSearchModel();
            var model = kitchensManager.GetDataTableDTO(searchModel, filters);

            return Json(model);
        }

        #endregion

        #region ایجاد

        public IActionResult LoadCreateForm()
        {
            ViewData["Org"] = new SelectList(webApiManager.GetOrganInfo(access_token), "Id", "UnitTitle");

            var model = new KitchenCreateDTO();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KitchenCreateDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    return Json(new
                    {
                        Status = false,
                        Message = string.Join("</br>", errors)
                    });
                }

                model.OrgTitle = webApiManager
                    .GetListOrganInfoV1(access_token)
                    .Where(x => x.Id == model.OrgId)
                    .Select(x => x.UnitTitle)
                    .FirstOrDefault();

                var res = kitchensManager.CreateKitchens(model);
                return Json(res);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ثبت اطلاعات با خطا همراه بوده است!"
                });
            }
        }

        #endregion

        #region ویرایش

        /// <summary>
        /// لود کردن فرم ویرایش در مدال
        /// </summary>
        /// <param name="id">شناسه آشپزخانه</param>
        /// <returns></returns>
        public IActionResult LoadEditForm(long id)
        {
            var model = kitchensManager.GetKitchenForEditDTO(id);

            if (model == null)
                return NotFound();

            ViewData["Org"] = new SelectList(webApiManager.GetOrganInfo(access_token), "Id", "UnitTitle", model.OrgId);

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(KitchenEditDTO model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "آشپزخانه مورد نظر یافت نشد!"
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    return Json(new
                    {
                        Status = false,
                        Message = string.Join("</br>", errors)
                    });
                }

                model.OrgTitle = webApiManager
                    .GetListOrganInfoV1(access_token)
                    .Where(x => x.Id == model.OrgId)
                    .Select(x => x.UnitTitle)
                    .FirstOrDefault();

                var res = kitchensManager.UpdateKitchens(model);
                return Json(res);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ویرایش اطلاعات با خطا همراه بوده است!"
                });
            }
        }

        #endregion

        #region حذف

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "اطلاعات مورد نظر یافت نشد!"
                    });
                }

                var isSuccess = kitchensManager.Delete(id);

                return Json(new
                {
                    Status = isSuccess,
                    Message = isSuccess ? "حذف گردید" : "حذف نگردید"
                });
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "حذف اطلاعات با خطا همراه بوده است!"
                });
            }
        }

        #endregion
    }
}