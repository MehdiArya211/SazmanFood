using BLL;
using BLL.FoodManag.DiningHallBL;
using BLL.Interface;
using Domain.Constants;
using Domain.Enums.Food;
using DTO.Entities;
using DTO.Entities.DiningHalDTo;
using Filters;
using ITOWebApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.SessionServices;
using Utilities.Extentions;

namespace Food.Areas.FoodMang.Controllers
{
    /// <summary>
    /// مدیریت سالن‌های غذاخوری
    /// </summary>
    [Area("FoodMang")]
    [UserAuthorize(
        Area: "FoodMang",
        Controller: "DiningHalls",
        Action: "index")]
    public class DiningHallsController : Controller
    {
        private readonly IDiningHallManager DiningHallManager;
        private readonly IPersonalManager PersonalManager;
        private readonly IDataTableManager dataTableManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession Session;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IWebApiManager webApiManager;

        private string access_token = string.Empty;

        public DiningHallsController(
            IHttpContextAccessor _httpContextAccessor,
            IDiningHallManager _DiningHallManager,
            IPersonalManager _PersonalManager,
            IDataTableManager _dataTableManager,
            ApiTokenCacheClient apiTokenCache,
            IWebApiManager _webApiManager)
        {
            httpContextAccessor =
                _httpContextAccessor;

            Session =
                httpContextAccessor.HttpContext.Session;

            DiningHallManager =
                _DiningHallManager;

            PersonalManager =
                _PersonalManager;

            dataTableManager =
                _dataTableManager;

            _apiTokenClient =
                apiTokenCache;

            webApiManager =
                _webApiManager;

            access_token = _apiTokenClient
                .GetApiToken(
                    CustomSettings.Instance.ClientId,
                    CustomSettings.Instance.Scope,
                    CustomSettings.Instance.ClientSecret,
                    CustomSettings.Instance.ROPC_UserName,
                    CustomSettings.Instance.ROPC_Password)
                .Result;
        }

        #region نمایش همه

        public IActionResult Index()
        {
            ViewData["PersonalType"] =
                new SelectList(
                    PersonalManager.GetSelectListDTO(),
                    "Id",
                    "Title");

            ViewData["UsageType"] =
                new SelectList(
                    EnumExtensions
                        .ToEnumViewModel<DiningHallUsageType>(),
                    "Id",
                    "Title");

            var user = Session.GetUser();
            var organizations =
                webApiManager.GetOrganInfo(access_token)
                    .Where(x =>
                        user == null ||
                        user.RoleId == RoleConstant.Admin ||
                        x.Id == user.OmdOrgId)
                    .ToList();

            ViewData["Org"] =
                new SelectList(
                    organizations,
                    "Id",
                    "UnitTitle");

            return View();
        }

        /// <summary>
        /// لیست اطلاعات موردنیاز DataTable
        /// </summary>
        [HttpPost]
        public ActionResult GetList(
            DiningHallFilterDTO filters)
        {
            var SearchModel =
                dataTableManager.GetSearchModel();

            filters ??=
                new DiningHallFilterDTO();

            var user =
                Session.GetUser();

            if (user != null &&
                user.RoleId != RoleConstant.Admin)
            {
                filters.OrgId =
                    user.OmdOrgId;
            }

            var model =
                DiningHallManager.GetDataTableDTO(
                    SearchModel,
                    filters);

            return Json(model);
        }

        #endregion

        #region ایجاد

        /// <summary>
        /// لود فرم ایجاد در مدال
        /// </summary>
        public IActionResult LoadCreateForm()
        {
            ViewData["PersonalTypeId"] =
                new SelectList(
                    PersonalManager.GetSelectListDTO(),
                    "Id",
                    "Title");


            var user = Session.GetUser();
            var organizations =
                webApiManager.GetOrganInfo(access_token)
                    .Where(x =>
                        user == null ||
                        user.RoleId == RoleConstant.Admin ||
                        x.Id == user.OmdOrgId)
                    .ToList();

            ViewData["Org"] =
                new SelectList(
                    organizations,
                    "Id",
                    "UnitTitle");

            var model = new DiningHallCreateDTO()
            {
                IsActive = true,
                IsInternal = false

            };

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            DiningHallCreateDTO model)
        {
            try
            {
                if (model.PersonalTypeId == 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "نوع پرسنل استفاده‌کننده را مشخص کنید!"
                    });
                }

                if (model.OrgId == 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "یگان را مشخص کنید!"
                    });
                }

                var currentUser =
                    Session.GetUser();

                if (currentUser == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "اطلاعات کاربر جاری یافت نشد!"
                    });
                }

                if (currentUser.RoleId != RoleConstant.Admin &&
                    model.OrgId != currentUser.OmdOrgId)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "شما مجاز به ثبت سالن برای یگان دیگر نیستید!"
                    });
                }

                if (!Enum.IsDefined(
                        typeof(DiningHallUsageType),
                        model.UsageType))
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "نوع استفاده از سالن را مشخص کنید!"
                    });
                }

                if (ModelState.IsValid)
                {
                    var organization =
                        webApiManager
                            .GetListOrganInfoV1(
                                access_token)
                            .FirstOrDefault(x =>
                                x.Id == model.OrgId);

                    if (organization == null)
                    {
                        return Json(new
                        {
                            Status = false,
                            Message =
                                "یگان انتخاب‌شده معتبر نیست!"
                        });
                    }

                    model.OrgTitle =
                        organization.UnitTitle;

                    var res =
                        DiningHallManager
                            .CreateDiningHalls(model);

                    return Json(res);
                }
                else
                {
                    var errors =
                        ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage);

                    return Json(new
                    {
                        Status = false,
                        Message = string.Join(
                            "</br>",
                            errors)
                    });
                }
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

        #endregion

        #region ویرایش

        /// <summary>
        /// لود فرم ویرایش در مدال
        /// </summary>
        public IActionResult LoadEditForm(long id)
        {
            var model =
                DiningHallManager
                    .GetDiningHallForEditDTO(id);

            if (model == null)
            {
                return NotFound();
            }

            model.HallCapacitySep =
                model.HallCapacity.HasValue
                    ? model.HallCapacity.Value.ToString("N0")
                    : null;

            ViewData["PersonalTypeId"] =
                new SelectList(
                    PersonalManager.GetSelectListDTO(),
                    "Id",
                    "Title",
                    model.PersonalTypeId);

            ViewData["UsageType"] =
                new SelectList(
                    EnumExtensions
                        .ToEnumViewModel<DiningHallUsageType>(),
                    "Id",
                    "Title",
                    (int)model.UsageType);

            var user = Session.GetUser();
            var organizations =
                webApiManager.GetOrganInfo(access_token)
                    .Where(x =>
                        user == null ||
                        user.RoleId == RoleConstant.Admin ||
                        x.Id == user.OmdOrgId)
                    .ToList();

            ViewData["Org"] =
                new SelectList(
                    organizations,
                    "Id",
                    "UnitTitle",
                    model.OrgId);

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            long id,
            DiningHallEditDTO model)
        {
            try
            {
                if (id != model.Id)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "سالن غذاخوری یافت نشد!"
                    });
                }

                if (model.PersonalTypeId == 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "نوع پرسنل استفاده‌کننده را مشخص کنید!"
                    });
                }

                if (model.OrgId == 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "یگان را مشخص کنید!"
                    });
                }

                var currentUser =
                    Session.GetUser();

                if (currentUser == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "اطلاعات کاربر جاری یافت نشد!"
                    });
                }

                if (currentUser.RoleId != RoleConstant.Admin &&
                    model.OrgId != currentUser.OmdOrgId)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "شما مجاز به ویرایش سالن یگان دیگر نیستید!"
                    });
                }

                if (!Enum.IsDefined(
                        typeof(DiningHallUsageType),
                        model.UsageType))
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            "نوع استفاده از سالن را مشخص کنید!"
                    });
                }

                if (ModelState.IsValid)
                {
                    var organization =
                        webApiManager
                            .GetListOrganInfoV1(
                                access_token)
                            .FirstOrDefault(x =>
                                x.Id == model.OrgId);

                    if (organization == null)
                    {
                        return Json(new
                        {
                            Status = false,
                            Message =
                                "یگان انتخاب‌شده معتبر نیست!"
                        });
                    }

                    model.OrgTitle =
                        organization.UnitTitle;

                    var res =
                        DiningHallManager
                            .UpdateDiningHall(model);

                    return Json(res);
                }
                else
                {
                    var errors =
                        ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage);

                    return Json(new
                    {
                        Status = false,
                        Message = string.Join(
                            "</br>",
                            errors)
                    });
                }
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
                        Message =
                            "سالن غذاخوری یافت نشد!"
                    });
                }

                var IsSuccess =
                    DiningHallManager.Delete(id);

                return Json(new
                {
                    Status = IsSuccess,

                    Message = IsSuccess
                        ? "سالن غذاخوری با موفقیت حذف شد."
                        : "حذف سالن غذاخوری با خطا همراه بوده است! ابتدا مطمئن شوید سالن در قسمت دیگری استفاده نشده باشد."
                });
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "حذف سالن غذاخوری با خطا همراه بوده است!"
                });
            }
        }

        #endregion
    }
}