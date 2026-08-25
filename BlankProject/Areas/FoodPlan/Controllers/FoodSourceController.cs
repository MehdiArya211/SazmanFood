using BLL;
using BLL.Interface;
using Domain.Enums;
using DTO.Entities;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.SessionServices;
using Utilities.Extentions;

namespace Food.Areas.FoodPlan.Controllers
{
    [Area("FoodPlan")]
    [UserAuthorize(
        Area: "FoodPlan",
        Controller: "FoodSource",
        Action: "index")]
    public class FoodSourceController : Controller
    {
        private readonly IFoodSourceManager foodSourceManager;
        private readonly IDataTableManager dataTableManager;
        private readonly ISession Session;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FoodSourceController(
            IHttpContextAccessor _httpContextAccessor,
            IFoodSourceManager _FoodSourceManager,
            IDataTableManager _dataTableManager)
        {
            foodSourceManager =
                _FoodSourceManager;

            dataTableManager =
                _dataTableManager;

            httpContextAccessor =
                _httpContextAccessor;

            Session =
                httpContextAccessor.HttpContext.Session;
        }

        public IActionResult Index()
        {
            LoadFilterItems();
            return View();
        }

        [HttpPost]
        public IActionResult GetList(
            FoodSourceFilterDataTableDTO filters)
        {
            var searchModel =
                dataTableManager.GetSearchModel();

            var model =
                foodSourceManager.GetDataTableDTO(
                    searchModel,
                    filters);

            return Json(model);
        }

        public IActionResult LoadCreateForm()
        {
            LoadFormItems();

            var model =
                new CreatFoodSourceDTO
                {
                    DayType = DayType.Normal
                };

            return PartialView(
                "_Create",
                model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            CreatFoodSourceDTO model)
        {
            try
            {
                var user =
                    Session.GetUser();

                if (user == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "اطلاعات کاربر جاری یافت نشد."
                    });
                }

                model.UserCreateId =
                    user.Id;

                if (!ModelState.IsValid)
                    return ModelStateError();

                var result =
                    foodSourceManager.Create(model);

                return Json(result);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "ثبت اطلاعات با خطا همراه بوده است!"
                });
            }
        }

        public IActionResult LoadEditForm(long id)
        {
            var model =
                foodSourceManager.GetEditDTO(id);

            if (model == null)
                return NotFound();

            LoadFormItems(
                model.YeganTypeId,
                model.PersonalTypeId,
                model.DayType);

            return PartialView(
                "_Edit",
                model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            EditFoodSourceDTO model)
        {
            try
            {
                if (model == null ||
                    model.Id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "مأخذ غذایی مورد نظر یافت نشد!"
                    });
                }

                var user =
                    Session.GetUser();

                if (user == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "اطلاعات کاربر جاری یافت نشد."
                    });
                }

                model.UserCreateId =
                    user.Id;

                if (!ModelState.IsValid)
                    return ModelStateError();

                var result =
                    foodSourceManager.Update(model);

                return Json(result);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "ویرایش اطلاعات با خطا همراه بوده است!"
                });
            }
        }

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
                        Message =
                            "مأخذ غذایی مورد نظر یافت نشد!"
                    });
                }

                var result =
                    foodSourceManager.Delete(id);

                return Json(new
                {
                    Status = result,
                    Message = result
                        ? "مأخذ غذایی با موفقیت حذف شد."
                        : "حذف مأخذ غذایی انجام نشد؛ ممکن است در سهمیه‌بندی استفاده شده باشد."
                });
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "حذف مأخذ غذایی با خطا همراه بوده است!"
                });
            }
        }

        private void LoadFilterItems()
        {
            ViewData["YeganTypes"] =
                new SelectList(
                    foodSourceManager
                        .GetSelectListYeganTypeDTO(),
                    "Id",
                    "Title");

            ViewData["PersonalTypes"] =
                new SelectList(
                    foodSourceManager
                        .GetSelectListPersonalTypeDTO(),
                    "Id",
                    "Title");

            ViewData["DayTypes"] =
                new SelectList(
                    EnumExtensions
                        .ToEnumViewModel<DayType>(),
                    "Id",
                    "Title");
        }

        private void LoadFormItems(
            long? yeganTypeId = null,
            long? personalTypeId = null,
            DayType? dayType = null)
        {
            ViewData["YeganTypes"] =
                new SelectList(
                    foodSourceManager
                        .GetSelectListYeganTypeDTO(),
                    "Id",
                    "Title",
                    yeganTypeId);

            ViewData["PersonalTypes"] =
                new SelectList(
                    foodSourceManager
                        .GetSelectListPersonalTypeDTO(),
                    "Id",
                    "Title",
                    personalTypeId);

            ViewData["DayTypes"] =
                new SelectList(
                    EnumExtensions
                        .ToEnumViewModel<DayType>(),
                    "Id",
                    "Title",
                    dayType.HasValue
                        ? (int)dayType.Value
                        : null);
        }

        private JsonResult ModelStateError()
        {
            var errors =
                ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .Distinct();

            return Json(new
            {
                Status = false,
                Message = string.Join(
                    "</br>",
                    errors)
            });
        }
    }
}