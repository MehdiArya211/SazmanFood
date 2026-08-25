using BLL.FoodManag.FoodBL;
using BLL.FoodManag.FoodTypesBl;
using BLL.Interface;
using DTO.Entities.FoodMang;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Food.Areas.FoodMang.Controllers
{
    /// <summary>
    /// تعریف غذا
    /// </summary>
    [Area("FoodMang")]
    [UserAuthorize(Area: "FoodMang", Controller: "FoodManages", Action: "index")]
    public class FoodManagesController : Controller
    {
        private readonly IFoodManager foodManager;
        private readonly IDataTableManager dataTableManager;
        private readonly IFoodTypesManager foodTypesManager;

        public FoodManagesController(
            IFoodManager _foodManager,
            IFoodTypesManager _foodTypesManager,
            IDataTableManager _dataTableManager)
        {
            foodManager = _foodManager;
            foodTypesManager = _foodTypesManager;
            dataTableManager = _dataTableManager;
        }

        #region نمایش

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetList(FoodFilterDTO filters)
        {
            var searchModel = dataTableManager.GetSearchModel();
            var model = foodManager.GetDataTableDTO(searchModel, filters);

            return Json(model);
        }

        #endregion

        #region ایجاد

        public IActionResult LoadCreateForm()
        {
            ViewData["FoodTypeList"] = new SelectList(foodTypesManager.GetAllFoodTypesListDTO(), "Id", "Title");

            var model = new FoodsCreateDTO();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FoodsCreateDTO model)
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

                var res = foodManager.CreateFoods(model);
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
        /// <param name="id">شناسه غذا</param>
        /// <returns></returns>
        public IActionResult LoadEditForm(long id)
        {
            var model = foodManager.GetFoodForEditDTO(id);

            if (model == null)
                return NotFound();

            ViewData["FoodTypeList"] = new SelectList(
                foodTypesManager.GetAllFoodTypesListDTO(),
                "Id",
                "Title",
                model.FoodTypesId);

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FoodEditDTO model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "غذای مورد نظر یافت نشد!"
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

                var res = foodManager.UpdateFoods(model);
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

                var isSuccess = foodManager.Delete(id);

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