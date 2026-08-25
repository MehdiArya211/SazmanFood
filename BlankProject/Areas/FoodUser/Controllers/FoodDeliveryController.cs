using BLL;
using DTO.Entities.QoutaPerson;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Food.Areas.FoodUser.Controllers
{
    [Area("FoodUser")]
    [UserAuthorize(Area: "FoodUser", Controller: "FoodDelivery", Action: "index")]
    public class FoodDeliveryController : Controller
    {
        private readonly IQoutaPersonManager qoutaPersonManager;
        private readonly IMealManager mealManager;

        public FoodDeliveryController(
            IQoutaPersonManager _qoutaPersonManager,
            IMealManager _mealManager)
        {
            qoutaPersonManager = _qoutaPersonManager;
            mealManager = _mealManager;
        }

        public IActionResult Index()
        {
            ViewData["Meals"] = new SelectList(mealManager.GetSelectListDTO(), "Id", "Title");

            var model = new DeliverFoodByCodeDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeliverByCode(DeliverFoodByCodeDTO model)
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

                var res = qoutaPersonManager.DeliverFoodByCode(model);

                return Json(new
                {
                    status = res.Status,
                    message = res.Message,
                    model = res.Model
                });
            }
            catch
            {
                return Json(new
                {
                    status = false,
                    message = "تحویل غذا با خطا همراه بوده است"
                });
            }
        }
    }
}