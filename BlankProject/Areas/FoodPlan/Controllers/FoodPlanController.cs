using BLL;
using BLL.Interface;
using DTO.Entities;
using Filters;
using ITOWebApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.RedisService;
using Services.SessionServices;

namespace Food.Areas.FoodPlan.Controllers
{
    /// <summary>
    /// برنامه غذایی
    /// </summary>
    [Area("FoodPlan")]
    [UserAuthorize(Area: "FoodPlan", Controller: "FoodPlan", Action: "index")]
    public class FoodPlanController : Controller
    {
        private readonly IFoodPlanManager foodPlanManager;
        private readonly IFoodPlanDayManager foodPlanDayManager;
        private readonly ISession Session;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IDataTableManager dataTableManager;
        private readonly IRedisManager Redis;
        private readonly ApiTokenCacheClient apiTokenClient;
        private readonly IWebApiManager webApiManager;

        private string access_token = string.Empty;

        public FoodPlanController(
            IRedisManager _Redis,
            IHttpContextAccessor _httpContextAccessor,
            IFoodPlanDayManager _FoodPlanDayManager,
            IFoodPlanManager _FoodPlanManager,
            IDataTableManager _dataTableManager,
            ApiTokenCacheClient _apiTokenClient,
            IWebApiManager _webApiManager)
        {
            Redis = _Redis;
            httpContextAccessor = _httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;

            foodPlanManager = _FoodPlanManager;
            foodPlanDayManager = _FoodPlanDayManager;
            dataTableManager = _dataTableManager;
            apiTokenClient = _apiTokenClient;
            webApiManager = _webApiManager;
            access_token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjkxRUQ1RDFGMEIxQzg3ODQ3NzE4QjMyNEQwQkM5QkU5IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3ODU0MDEyNDgsImV4cCI6MTc4NTQwNDg0OCwiaXNzIjoiaHR0cDovL2l0b2lkZW50aXR5c2VydmVyLm5lei5uZXQiLCJhdWQiOlsiT3JnYW5BcGkiLCJQZXJzb25lbEFwaSIsIlByb3ZpbmNlQXBpIl0sImNsaWVudF9pZCI6IkRlcHJpdmF0aW9uIiwic3ViIjoiZGVwcml2YXRpb24iLCJhdXRoX3RpbWUiOjE3ODU0MDEyNDgsImlkcCI6ImxvY2FsIiwianRpIjoiMUJCM0FEQzIwNjg0NDYxNDNFOTkyMzM3M0RENUJGMkUiLCJpYXQiOjE3ODU0MDEyNDgsInNjb3BlIjpbIm9yZ2FuLmluZm8iLCJwZXJzb25hbC5pbmZvIiwicHJvdmluY2UuaW5mbyJdLCJhbXIiOlsiY3VzdG9tIl19.GcBFFzII5Q66M35Rr2Mk6_FWPE-YihRJco5TDz3q91vjOvO4_KpemZDtQsX3o9SeplTTls-mjEeLWxmkBD6f56fnsGyGNkDFSK5yPZ_C3B83413r_E1s2mOu8yU7yeDznMH5sagFRH5BQX32Kw3tk2mO-vgIXdscr2VvQZmnDPfw_K0Z9HSiJt6VAEN_9jdYsrZoInvjAyDBSSYvdSTQGjHCAbcSGmMNgScfUB4IjlwD1xhMMmt_WiNHupRI7QwXZ4WGRiN1oNtrU-1T7GnlKukVL1kR4_V3uR_ZmT5UwZnSCbtq8hxv9tOK3UqAEM5kucGxYtldeCj8jmWXnoXp7g";

            //access_token = apiTokenClient.GetApiToken(
            //    CustomSettings.Instance.ClientId,
            //    CustomSettings.Instance.Scope,
            //    CustomSettings.Instance.ClientSecret,
            //    CustomSettings.Instance.ROPC_UserName,
            //    CustomSettings.Instance.ROPC_Password
            //).Result;
        }

        #region نمایش همه

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// لیست داده مورد نیاز برای دیتاتیبل
        /// </summary>
        /// <param name="filters">فیلترهای برنامه غذایی</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetList(FoodPlanFilterDataTableDTO filters)
        {
            var searchModel = dataTableManager.GetSearchModel();
            var model = foodPlanManager.GetDataTableDTO(searchModel, filters);

            return Json(model);
        }

        #endregion

        #region ایجاد

        /// <summary>
        /// لود کردن فرم ایجاد در مدال
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadCreateForm()
        {
            ViewData["Organ"] = new SelectList(webApiManager.GetOrganInfo(access_token), "Id", "UnitTitle");
            ViewData["Season"] = new SelectList(foodPlanManager.GetSelectListSeasonDTO(), "Id", "Title");
            ViewData["Years"] = new SelectList(foodPlanManager.GetSelectListYearsDTO(), "Id", "Title");

            var model = new CreatFoodPlanDTO();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreatFoodPlanDTO model)
        {
            try
            {
                var user = Session.GetUser();
                model.UserCreateId = user.Id;

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

                var organ = webApiManager.GetOrganInfoByOrganId(model.OrganId, access_token);
                model.OrganTitle = organ?.UnitTitle;

                var res = foodPlanManager.Create(model);
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
        /// <param name="id">شناسه برنامه غذایی</param>
        /// <returns></returns>
        public IActionResult LoadEditForm(long id)
        {
            var model = foodPlanManager.GetEditDTO(id);

            if (model == null)
                return NotFound();

            ViewData["Organ"] = new SelectList(webApiManager.GetOrganInfo(access_token), "Id", "UnitTitle", model.OrganId);
            ViewData["Season"] = new SelectList(foodPlanManager.GetSelectListSeasonDTO(), "Id", "Title", model.SeasonId);
            ViewData["Years"] = new SelectList(foodPlanManager.GetSelectListYearsDTO(), "Id", "Title", model.YearsId);

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditFoodPlanDTO model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "برنامه غذایی مورد نظر یافت نشد!"
                    });
                }

                var user = Session.GetUser();
                model.UserCreateId = user.Id;

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

                var organ = webApiManager.GetOrganInfoByOrganId(model.OrganId, access_token);
                model.OrganTitle = organ?.UnitTitle;

                var res = foodPlanManager.Update(model);
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

        #region حذف برنامه غذایی

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
                        Message = "برنامه غذایی مورد نظر یافت نشد!"
                    });
                }

                var listFoodPlanDay = foodPlanDayManager
                    .GetAll()
                    .Where(d => d.FoodPlanId == id)
                    .ToList();

                if (listFoodPlanDay.Any())
                    foodPlanDayManager.DeleteRange(listFoodPlanDay);

                var res = foodPlanManager.Delete(id);

                return Json(new
                {
                    Status = res,
                    Message = res
                        ? "برنامه غذایی با موفقیت حذف شد"
                        : "حذف برنامه غذایی با خطا همراه بوده است! ابتدا مطمئن شوید که این برنامه غذایی در جای دیگری از سایت مورد استفاده قرار نگرفته است!"
                });
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "حذف برنامه غذایی با خطا همراه بوده است!"
                });
            }
        }

        #endregion

        #region افزودن وعده غذایی

        /// <summary>
        /// لیست وعده‌های غذایی برنامه
        /// </summary>
        /// <param name="id">شناسه برنامه غذایی</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetListFoodPlanDay(long id)
        {
            var searchModel = dataTableManager.GetSearchModel();
            var model = foodPlanDayManager.GetFoodPlanDayDataTableDTO(searchModel, id);

            return Json(model);
        }

        public IActionResult LoadCreateFormAddFood(long foodPlanId)
        {
            FillFoodPlanDayViewData(foodPlanId, null);

            var model = new FoodPlanDayDataTableDTO
            {
                FoodPlanId = foodPlanId
            };

            return PartialView("AddFoodPlanDay", model);
        }

        public IActionResult LoadEditFoodPlanDay(long foodPlanDayId, long foodPlanId)
        {
            FillFoodPlanDayViewData(foodPlanId, foodPlanDayId);

            var model = foodPlanDayManager.GetFoodPlanDayEditDTO(foodPlanDayId);

            if (model == null)
                return NotFound();

            model.FoodPlanId = foodPlanId;
            model.FoodPlanDayId = foodPlanDayId;

            return PartialView("AddFoodPlanDay", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAddFood(FoodPlanDayDataTableDTO model)
        {
            try
            {
                var user = Session.GetUser();
                model.UserCreateId = user.Id;

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

                if (model.FoodPlanId <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "شناسه برنامه غذایی نامعتبر است"
                    });
                }

                if (model.FoodPlanDayId == 0)
                {
                    var res = foodPlanDayManager.AddFoodPlanDay(model);

                    return Json(new
                    {
                        Status = res.Status,
                        Message = res.Status ? "وعده غذایی با موفقیت افزوده شد" : res.Message
                    });
                }
                else
                {
                    var res = foodPlanDayManager.UpdateFoodPlanDay(model);

                    return Json(new
                    {
                        Status = res.Status,
                        Message = res.Status ? "وعده غذایی با موفقیت ویرایش شد" : res.Message
                    });
                }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFoodPlanDay(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "وعده غذایی مورد نظر یافت نشد!"
                    });
                }

                var res = foodPlanDayManager.Delete(id);

                return Json(new
                {
                    Status = res,
                    Message = res
                        ? "وعده غذایی با موفقیت حذف شد"
                        : "حذف وعده غذایی با خطا همراه بوده است! ابتدا مطمئن شوید که این وعده غذایی در جای دیگری از سایت مورد استفاده قرار نگرفته است!"
                });
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "حذف وعده غذایی با خطا همراه بوده است!"
                });
            }
        }

        /// <summary>
        /// مقداردهی ViewData های فرم افزودن/ویرایش وعده غذایی
        /// </summary>
        /// <param name="foodPlanId">شناسه برنامه غذایی</param>
        /// <param name="foodPlanDayId">شناسه برنامه غذایی روز</param>
        private void FillFoodPlanDayViewData(long foodPlanId, long? foodPlanDayId)
        {
            ViewData["Meals"] = new SelectList(foodPlanDayManager.GetSelectListMealDTO(), "Id", "Title");
            ViewData["Days"] = new SelectList(foodPlanDayManager.GetSelectListDaysDTO(), "Id", "Title");
            ViewData["Foods"] = new SelectList(foodPlanDayManager.GetSelectListFoodDTO(), "Id", "Title");
            ViewData["FoodDesser"] = new SelectList(foodPlanDayManager.GetSelectListFoodDesserDTO(), "Id", "Title");
            ViewData["FoodDorchin"] = new SelectList(foodPlanDayManager.GetSelectListFoodDorchinDTO(), "Id", "Title");
            ViewData["FoodPlanId"] = foodPlanId;
            ViewData["FoodPlanDayId"] = foodPlanDayId;
        }

        #endregion
    }
}