using BLL;
using BLL.Interface;
using DTO.Entities;
using DTO.Entities.QoutaPerson;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.SessionServices;

namespace Food.Areas.FoodUser.Controllers
{
    /// <summary>
    /// غذای من
    /// </summary>
    [Area("FoodUser")]
    [UserAuthorize(Area: "FoodUser", Controller: "MyFood", Action: "index")]
    public class MyFoodController : Controller
    {
        private readonly IQoutaPersonManager qoutaPersonManager;
        private readonly IFoodPlanDayManager foodPlanDayManager;
        private readonly IDataTableManager dataTableManager;
        private readonly ISession Session;

        public MyFoodController(
            IQoutaPersonManager _qoutaPersonManager,
            IFoodPlanDayManager _foodPlanDayManager,
            IDataTableManager _dataTableManager,
            IHttpContextAccessor httpContextAccessor)
        {
            qoutaPersonManager = _qoutaPersonManager;
            foodPlanDayManager = _foodPlanDayManager;
            dataTableManager = _dataTableManager;
            Session = httpContextAccessor.HttpContext.Session;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetList()
        {
            var user = Session.GetUser();

            if (user.PersonId == null)
            {
                return Json(new
                {
                    draw = Request.Form["draw"].FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>()
                });
            }

            var searchModel = dataTableManager.GetSearchModel();
            var model = qoutaPersonManager.GetMyFoodDataTableDTO(searchModel, user.PersonId.Value);

            return Json(model);
        }

        public IActionResult LoadChangeFoodForm(long qoutaPersonId)
        {
            ViewData["Foods"] = new SelectList(foodPlanDayManager.GetSelectListFoodDTO(), "Id", "Title");
            ViewData["Desserts"] = new SelectList(foodPlanDayManager.GetSelectListFoodDesserDTO(), "Id", "Title");
            ViewData["SideDishes"] = new SelectList(foodPlanDayManager.GetSelectListFoodDorchinDTO(), "Id", "Title");

            var model = new ChangePersonFoodDTO
            {
                QoutaPersonId = qoutaPersonId
            };

            return PartialView("_ChangeFood", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeFood(ChangePersonFoodDTO model)
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
                        status = false,
                        message = string.Join("</br>", errors)
                    });
                }

                var res = qoutaPersonManager.ChangePersonFood(model);

                return Json(new
                {
                    status = res.Status,
                    message = res.Message
                });
            }
            catch
            {
                return Json(new
                {
                    status = false,
                    message = "تغییر غذا با خطا همراه بوده است"
                });
            }
        }
    }
}