using BLL;
using BLL.Garrision;
using BLL.Interface;
using DTO;
using DTO.Entities;
using DTO.Entities.Garrison;
using DTO.Entities.QoutaPerson;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Food.Areas.Qouta.Controllers;

/// <summary>
/// مدیریت سهمیه بندی
/// </summary>
[Area("Qouta")]
[UserAuthorize(Area: "Qouta", Controller: "QoutaAllocation", Action: "index")]
public class QoutaAllocationController : Controller
{
    private readonly IYearsManager _yearsManager;
    private readonly IQoutaAllocationManager _qoutaAllocationManager;
    private readonly IQoutaPersonManager _qoutaPersonManager;
    private readonly IOrganGarrisionManager _organGarrisionManager;
    private readonly IDataTableManager _dataTableManager;
    private readonly IPersonalManager _personalManager;
    private readonly IFoodPlanDayManager _foodPlanDayManager;
    private readonly IMealManager _mealManager;

    public QoutaAllocationController(
        IQoutaAllocationManager qoutaManager,
        IQoutaPersonManager qoutaPersonManager,
        IOrganGarrisionManager organGarrisionManager,
        IDataTableManager dataTableManager,
        IPersonalManager personalManager,
        IFoodPlanDayManager foodPlanDayManager,
        IMealManager mealManager,
        IYearsManager yearsManager)
    {
        _qoutaAllocationManager = qoutaManager;
        _qoutaPersonManager = qoutaPersonManager;
        _organGarrisionManager = organGarrisionManager;
        _dataTableManager = dataTableManager;
        _personalManager = personalManager;
        _foodPlanDayManager = foodPlanDayManager;
        _mealManager = mealManager;
        _yearsManager = yearsManager;
    }

    #region نمایش همه

    public IActionResult Index()
    {
        FillBaseViewData();
        return View();
    }

    /// <summary>
    /// لیست داده مورد نیاز برای دیتاتیبل
    /// </summary>
    [HttpPost]
    public IActionResult GetList(QoutaAllocationFilterDataTableDTO filters)
    {
        var searchModel = _dataTableManager.GetSearchModel();
        var model = _qoutaAllocationManager.GetDataTableDTO(searchModel, filters);

        return Json(model);
    }

    #endregion

    #region ایجاد

    /// <summary>
    /// لود کردن فرم ایجاد در مدال
    /// </summary>
    public IActionResult LoadCreateForm()
    {
        FillBaseViewData();

        var model = new QoutaAllocationCreateDTO();
        return PartialView("_Create", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(QoutaAllocationCreateDTO model, DateTime RegQoutaAllocationDate)
    {
        try
        {
            model.QoutaAllocationDate = RegQoutaAllocationDate;

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

            var res = _qoutaAllocationManager.Create(model);
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

    public IActionResult GetOrganGarrison(long id)
    {
        var organGarrisonList = _organGarrisionManager.GetSubSelectListDTO(id);
        return Json(organGarrisonList);
    }

    #endregion

    #region افزودن نفرات سهمیه بندی

    [HttpPost]
    public IActionResult GetListQoutaPerson(long id)
    {
        var searchModel = _dataTableManager.GetSearchModel();
        var model = _qoutaPersonManager.GetSelectablePersonsForQuota(searchModel, id);

        return Json(model);
    }

    /// <summary>
    /// لود کردن فرم افزودن نفرات سهمیه بندی
    /// </summary>
    public IActionResult LoadCreateFormAddPerson(long id)
    {
        var qoutaAllocationId = id;

        var qoutaAllocation = _qoutaAllocationManager.GetById(qoutaAllocationId);
        if (qoutaAllocation == null)
            return NotFound();

        var model = new QoutaPersonCreateDTO
        {
            QoutaAllocationId = qoutaAllocationId
        };

        var sahmiye = _qoutaPersonManager.GetSahmiye(qoutaAllocationId);
        model.EstedadeKadr = sahmiye.EstedadeKadr;
        model.EstedadeVazife = sahmiye.EstedadeVazife;
        model.MealTitle = sahmiye.MealTitle;

        model.Personels = _qoutaPersonManager.GetPersonListWithQouataAllocation(qoutaAllocationId)
                         ?? new List<QoutaPersonDataTableDTO>();

        var activeYearId = _yearsManager.GetActiveYearId();
        if (!activeYearId.HasValue)
        {
            ViewBag.Error = "سال فعال در سیستم تعریف نشده است.";
            ViewBag.AvailableMainFoods = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.AvailableMainFoodsJson = new List<object>();
            model.Foods = new List<FoodPlanDayListForSahmiyebandiDTO>();

            return PartialView("_AddPerson", model);
        }

        var foods = _foodPlanDayManager.GetAllFoodForSahmiyebandikadrandvazifeh(
            qoutaAllocation.DayId,
            qoutaAllocation.MealId,
            activeYearId.Value
        ) ?? new List<FoodPlanDayListForSahmiyebandiDTO>();

        model.Foods = foods;

        var foodsForSelect = foods
            .GroupBy(x => x.FoodId)
            .Select(g => g.First())
            .OrderBy(x => x.FoodTitle)
            .ToList();

        ViewBag.AvailableMainFoods = new SelectList(foodsForSelect, "FoodId", "FoodTitle");

        ViewBag.AvailableMainFoodsJson = foodsForSelect
            .Select(x => new
            {
                value = x.FoodId,
                text = x.FoodTitle
            })
            .ToList();

        if (!foodsForSelect.Any())
            ViewBag.Error = "برای این روز/وعده غذایی در برنامه غذایی فعال، غذایی یافت نشد.";

        return PartialView("_AddPerson", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAddPerson(QoutaPersonCreateDTO model, long QoutaAllocationId)
    {
        try
        {
            model.QoutaAllocationId = QoutaAllocationId;

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

            var res = _qoutaPersonManager.Create(model);

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
                message = "ثبت اطلاعات با خطا همراه بوده است!"
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAddPersonBulk([FromBody] QoutaPersonBulkCreateDTO model)
    {
        try
        {
            if (model == null || model.QoutaAllocationId <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسالی نامعتبر است"
                });
            }

            var res = _qoutaPersonManager.CreateBulk(model);

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
                message = "ثبت گروهی با خطا همراه بوده است!"
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAddPersonSingle(long QoutaAllocationId, long PersonId, long FoodId, long FoodTokenTypeId = 1)
    {
        try
        {
            if (QoutaAllocationId <= 0 || PersonId <= 0 || FoodId <= 0)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسالی ناقص است"
                });
            }

            var model = new QoutaPersonOfficeCreateDTO
            {
                QoutaAllocationId = QoutaAllocationId,
                PersonId = PersonId,
                FoodTokenTypeId = FoodTokenTypeId,
                MainFoodId = FoodId
            };

            var res = _qoutaPersonManager.CreateForOfficeUser(model);

            return Json(new
            {
                status = res.Status,
                message = res.Message ?? (res.Status ? "با موفقیت ثبت شد!" : "ثبت با خطا مواجه شد.")
            });
        }
        catch
        {
            return Json(new
            {
                status = false,
                message = "ثبت اطلاعات با خطا همراه بوده است!"
            });
        }
    }

    #endregion

    #region ویرایش

    public IActionResult LoadEditForm(long id)
    {
        var qoutaAllocation = _qoutaAllocationManager.GetEditDTO(id);
        if (qoutaAllocation == null)
            return NotFound();

        FillBaseViewData();

        if (qoutaAllocation.OrganGarrisonParentId.HasValue)
        {
            ViewData["OrganGarrisonSub"] = new SelectList(
                _organGarrisionManager.GetSubSelectListDTO(qoutaAllocation.OrganGarrisonParentId.Value),
                "Id",
                "Title",
                qoutaAllocation.OrganGarrisonId);
        }
        else
        {
            ViewData["OrganGarrisonSub"] = new SelectList(Enumerable.Empty<SelectListItem>());
        }

        return PartialView("_Edit", qoutaAllocation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(QoutaAllocationEditDTO model)
    {
        try
        {
            if (model == null || model.Id <= 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "سهمیه بندی مورد نظر یافت نشد!"
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

            var res = _qoutaAllocationManager.Update(model);
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
                    Message = "سهمیه بندی مورد نظر یافت نشد!"
                });
            }

            var isSuccess = _qoutaAllocationManager.Delete(id);

            return Json(new
            {
                Status = isSuccess,
                Message = isSuccess ? "با موفقیت حذف شد" : "حذف با خطا همراه بوده است!"
            });
        }
        catch
        {
            return Json(new
            {
                Status = false,
                Message = "حذف با خطا همراه بوده است!"
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePerson(long id)
    {
        try
        {
            if (id <= 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "رکورد مورد نظر یافت نشد!"
                });
            }

            var isSuccess = _qoutaPersonManager.Delete(id);

            return Json(new
            {
                Status = isSuccess,
                Message = isSuccess ? "با موفقیت حذف شد" : "حذف با خطا همراه بوده است!"
            });
        }
        catch
        {
            return Json(new
            {
                Status = false,
                Message = "حذف با خطا همراه بوده است!"
            });
        }
    }

    #endregion

    #region Helper

    private void FillBaseViewData()
    {
        ViewData["OrganGarrison"] = new SelectList(_organGarrisionManager.GetSelectListDTO(), "Id", "Title");
        ViewData["Days"] = new SelectList(_foodPlanDayManager.GetSelectListDTO(), "Id", "Title");
        ViewData["Meals"] = new SelectList(_mealManager.GetSelectListDTO(), "Id", "Title");
        ViewData["personalType"] = new SelectList(_personalManager.GetSelectListDTO(), "Id", "Title");
    }

    #endregion


    public IActionResult LoadCreateGuestFood(long qoutaAllocationId)
    {
        var qoutaAllocation = _qoutaAllocationManager.GetById(qoutaAllocationId);
        if (qoutaAllocation == null)
            return NotFound();

        var activeYearId = _yearsManager.GetActiveYearId();
        if (!activeYearId.HasValue)
        {
            ViewBag.Error = "سال فعال در سیستم تعریف نشده است.";
            ViewBag.AvailableMainFoods = new SelectList(Enumerable.Empty<SelectListItem>());
        }
        else
        {
            var foods = _foodPlanDayManager.GetAllFoodForSahmiyebandikadrandvazifeh(
                qoutaAllocation.DayId,
                qoutaAllocation.MealId,
                activeYearId.Value
            ) ?? new List<FoodPlanDayListForSahmiyebandiDTO>();

            var foodsForSelect = foods
                .GroupBy(x => x.FoodId)
                .Select(g => g.First())
                .OrderBy(x => x.FoodTitle)
                .ToList();

            ViewBag.AvailableMainFoods = new SelectList(foodsForSelect, "FoodId", "FoodTitle");
        }

        var model = new GuestFoodCreateDTO
        {
            QoutaAllocationId = qoutaAllocationId,
            Count = 1,
            FoodTokenTypeId = 1
        };

        return PartialView("_AddGuest", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateGuestFood(GuestFoodCreateDTO model)
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

            var res = _qoutaPersonManager.CreateGuestFood(model);

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
                message = "ثبت غذای مهمان با خطا همراه بوده است!"
            });
        }
    }

    public IActionResult GetCapacityStatus(long qoutaAllocationId)
    {
        var model = _qoutaPersonManager.GetCapacityStatus(qoutaAllocationId);

        if (model == null)
        {
            return Json(new
            {
                status = false,
                message = "اطلاعات ظرفیت یافت نشد"
            });
        }

        return Json(new
        {
            status = true,
            model
        });
    }


    public IActionResult PrintToken(long qoutaPersonId)
    {
        var model = _qoutaPersonManager.GetById(qoutaPersonId);

        if (model == null)
            return NotFound();

        return View("PrintToken", model);
    }
}