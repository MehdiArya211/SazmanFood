using BLL;
using BLL.Interface;
using Domain.Constants;
using DTO.Entities;
using DTO.User;
using Filters;
using ITOWebApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.SessionServices;
using Utilities.Extentions;

namespace Food.Areas.Qouta.Controllers;


/// <summary>
/// نمایش سهمیه‌بندی و ثبت نفرات
/// </summary>

[Area("Qouta")]
[UserAuthorize(Area: "Qouta", Controller: "UnitQuotas", Action: "index")]
public class UnitQuotasController : Controller
{
private readonly IUnitQuotaManager unitQuotaManager;
    private readonly IWebApiManager webApiManager;
    private readonly ISession Session;
    private readonly ApiTokenCacheClient apiTokenClient;
    private string access_token = string.Empty;

    public UnitQuotasController(
        IHttpContextAccessor httpContextAccessor,
        IUnitQuotaManager unitQuotaManager,
        IWebApiManager webApiManager,
        ApiTokenCacheClient apiTokenClient)
    {
        this.unitQuotaManager =
            unitQuotaManager ??
            throw new ArgumentNullException(
                nameof(unitQuotaManager));

        this.webApiManager =
            webApiManager ??
            throw new ArgumentNullException(
                nameof(webApiManager));

        this.apiTokenClient =
            apiTokenClient ??
            throw new ArgumentNullException(
                nameof(apiTokenClient));

        Session =
            httpContextAccessor?
                .HttpContext?
                .Session;

        // access_token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjkxRUQ1RDFGMEIxQzg3ODQ3NzE4QjMyNEQwQkM5QkU5IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3ODU0MDEyNDgsImV4cCI6MTc4NTQwNDg0OCwiaXNzIjoiaHR0cDovL2l0b2lkZW50aXR5c2VydmVyLm5lei5uZXQiLCJhdWQiOlsiT3JnYW5BcGkiLCJQZXJzb25lbEFwaSIsIlByb3ZpbmNlQXBpIl0sImNsaWVudF9pZCI6IkRlcHJpdmF0aW9uIiwic3ViIjoiZGVwcml2YXRpb24iLCJhdXRoX3RpbWUiOjE3ODU0MDEyNDgsImlkcCI6ImxvY2FsIiwianRpIjoiMUJCM0FEQzIwNjg0NDYxNDNFOTkyMzM3M0RENUJGMkUiLCJpYXQiOjE3ODU0MDEyNDgsInNjb3BlIjpbIm9yZ2FuLmluZm8iLCJwZXJzb25hbC5pbmZvIiwicHJvdmluY2UuaW5mbyJdLCJhbXIiOlsiY3VzdG9tIl19.GcBFFzII5Q66M35Rr2Mk6_FWPE-YihRJco5TDz3q91vjOvO4_KpemZDtQsX3o9SeplTTls-mjEeLWxmkBD6f56fnsGyGNkDFSK5yPZ_C3B83413r_E1s2mOu8yU7yeDznMH5sagFRH5BQX32Kw3tk2mO-vgIXdscr2VvQZmnDPfw_K0Z9HSiJt6VAEN_9jdYsrZoInvjAyDBSSYvdSTQGjHCAbcSGmMNgScfUB4IjlwD1xhMMmt_WiNHupRI7QwXZ4WGRiN1oNtrU-1T7GnlKukVL1kR4_V3uR_ZmT5UwZnSCbtq8hxv9tOK3UqAEM5kucGxYtldeCj8jmWXnoXp7g";

        access_token =
            apiTokenClient.GetApiToken(
                CustomSettings.Instance.ClientId,
                CustomSettings.Instance.Scope,
                CustomSettings.Instance.ClientSecret,
                CustomSettings.Instance.ROPC_UserName,
                CustomSettings.Instance.ROPC_Password
            ).Result;
    }

    #region دسترسی‌ها

    private UserSessionDTO GetCurrentUser()
    {
        return Session?.GetUser();
    }

    private bool CanViewAllOrganizations()
    {
        var user = GetCurrentUser();
        return user?.IsEnabled == true &&
               (user.RoleId == RoleConstant.Admin || user.RoleId == RoleConstant.FoodSupport);
    }

    private bool CanView()
    {
        var user = GetCurrentUser();

        if (user == null || !user.IsEnabled)
            return false;

        return user.OmdOrgId > 0 ||
               CanViewAllOrganizations();
    }

    private bool CanManageOrganization(int orgId)
    {
        var user = GetCurrentUser();

        if (user == null || !user.IsEnabled)
            return false;

        /*
         * رکن 4 همه یگان‌ها را مشاهده می‌کند،
         * اما ثبت و حذف فقط برای یگان خود کاربر است.
         */
        return user.RoleId == RoleConstant.Admin ||
               (user.OmdOrgId > 0 && user.OmdOrgId == orgId);
    }

    private IActionResult AccessDenied(
        string message =
            "شما مجوز انجام این عملیات را ندارید.")
    {
        return StatusCode(
            StatusCodes.Status403Forbidden,
            new
            {
                Status = false,
                Message = message
            });
    }

    private IActionResult ModelStateError()
    {
        var errors =
            ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage);

        return Json(new
        {
            Status = false,
            Message = string.Join(
                "</br>",
                errors)
        });
    }

    #endregion

    #region نمایش

    public IActionResult Index()
    {
        if (!CanView())
            return AccessDenied();

        var user = GetCurrentUser();
        var canViewAll = CanViewAllOrganizations();

        ViewBag.CanViewAllOrganizations = canViewAll;
        ViewBag.CurrentOrgId = user?.OmdOrgId ?? 0;

        if (canViewAll)
        {
            ViewData["Organizations"] = new SelectList(
                webApiManager.GetOrganInfo(access_token),
                "Id",
                "UnitTitle");
        }

        return View();
    }

    [HttpPost]
    public IActionResult GetList(
        UnitQuotaFilterDTO filters)
    {
        try
        {
            if (!CanView())
                return AccessDenied();

            var canViewAll =
                CanViewAllOrganizations();

            filters ??=
                new UnitQuotaFilterDTO();

            /*
             * کاربر عادی نمی‌تواند OrgId را دست‌کاری کند.
             */
            if (!canViewAll)
            {
                var user =
                    GetCurrentUser();

                if (user == null ||
                    user.OmdOrgId <= 0)
                {
                    return AccessDenied(
                        "یگان کاربر مشخص نشده است.");
                }

                filters.OrgId =
                    user.OmdOrgId;
            }

            var model =
                unitQuotaManager.GetList(
                    filters,
                    canViewAll);

            return Json(new
            {
                data = model
            });
        }
        catch
        {
            return Json(new
            {
                data =
                    new List<UnitQuotaDTO>(),

                Status = false,
                Message =
                    "دریافت سهمیه‌ها با خطا همراه بوده است."
            });
        }
    }

    #endregion

    #region فرم ثبت کادر

    public IActionResult LoadOfficialForm(
        long unitQuotaId,
        long mealId)
    {
        if (!CanView())
            return AccessDenied();

        var model = unitQuotaManager.GetOfficialForm(
            unitQuotaId,
            mealId);

        if (model == null)
        {
            return NotFound(new
            {
                Status = false,
                Message = "سهمیه کادر مورد نظر یافت نشد."
            });
        }

        if (!CanManageOrganization(model.OrgId))
        {
            return AccessDenied(
                "شما اجازه ثبت کادر برای این یگان را ندارید.");
        }

        ViewBag.DiningHalls = unitQuotaManager
            .GetDiningHalls(
                model.OrgId,
                model.PersonalTypeId)
            .ToList();

        return PartialView("_Official", model);
    }

    #endregion

    #region جست‌وجوی کادر

    [HttpGet]
    public IActionResult SearchOfficial(
        string personCode)
    {
        try
        {
            if (!CanView())
                return AccessDenied();

            var user =
                GetCurrentUser();

            if (user == null)
            {
                return AccessDenied(
                    "اطلاعات کاربر در سشن یافت نشد.");
            }

            personCode =
                personCode?
                    .Trim()
                    .ToEnglishNumber();

            if (string.IsNullOrWhiteSpace(
                    personCode))
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "کد پرسنلی را وارد کنید."
                });
            }

            var person =
                webApiManager
                    .GetPersonalByPersonCode(
                        personCode,
                        access_token);

            if (person == null)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "پرسنلی با این کد پرسنلی یافت نشد."
                });
            }

            /*
             * جست‌وجوی کادر فقط برای یگان خود کاربر.
             */
            if (user.RoleId != RoleConstant.Admin && person.UnitCode !=
                user.OmdOrgId)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "این پرسنل متعلق به یگان شما نیست."
                });
            }

            var fullName =
                !string.IsNullOrWhiteSpace(
                    person.FullName)
                    ? person.FullName.Trim()
                    : $"{person.FirstName} {person.LastName}"
                        .Trim();

            return Json(new
            {
                Status = true,

                Model = new
                {
                    PersonId =
                        person.Id,

                    PersonCode =
                        person.personalCode,

                    NationalCode =
                        person.MelliCode,

                    FullName =
                        fullName,

                    RankTitle =
                        person.RankTitle,

                    UnitCode =
                        person.UnitCode,

                    UnitTitle =
                        person.UnitTitle
                }
            });
        }
        catch
        {
            return Json(new
            {
                Status = false,
                Message =
                    "دریافت اطلاعات پرسنل با خطا همراه بوده است."
            });
        }
    }

    #endregion

    #region ثبت کادر

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateOfficial(
        UnitQuotaOfficialCreateDTO model)
    {
        try
        {
            if (!CanView())
                return AccessDenied();

            if (model == null)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "اطلاعات ارسالی معتبر نیست."
                });
            }

            /*
             * اطلاعات دریافتی وب‌سرویس از ModelState
             * فرم حذف می‌شوند چون از فرم قابل اعتماد نیستند.
             */
            ModelState.Remove(
                nameof(model.PersonId));

            ModelState.Remove(
                nameof(model.NationalCode));

            ModelState.Remove(
                nameof(model.FullName));

            ModelState.Remove(
                nameof(model.RankTitle));

            if (!ModelState.IsValid)
                return ModelStateError();

            var personCode =
                model.PersonCode?
                    .Trim()
                    .ToEnglishNumber();

            if (string.IsNullOrWhiteSpace(
                    personCode))
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "کد پرسنلی معتبر نیست."
                });
            }

            /*
             * هنگام ذخیره مجدداً وب‌سرویس فراخوانی می‌شود
             * تا اطلاعات مخفی فرم قابل دست‌کاری نباشد.
             */
            var person =
                webApiManager
                    .GetPersonalByPersonCode(
                        personCode,
                        access_token);

            if (person == null)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "پرسنلی با این کد پرسنلی یافت نشد."
                });
            }

            var result =
                unitQuotaManager.CreateOfficial(
                    model,
                    person);

            return Json(result);
        }
        catch
        {
            return Json(new
            {
                Status = false,
                Message =
                    "ثبت کارکنان کادر با خطا همراه بوده است."
            });
        }
    }

    #endregion

    #region فرم ثبت وظیفه

    public IActionResult LoadDutyForm(
        long unitQuotaId,
        long mealId)
    {
        if (!CanView())
            return AccessDenied();

        var model = unitQuotaManager.GetDutyForm(
            unitQuotaId,
            mealId);

        if (model == null)
        {
            return NotFound(new
            {
                Status = false,
                Message = "سهمیه وظیفه مورد نظر یافت نشد."
            });
        }

        if (!CanManageOrganization(model.OrgId))
        {
            return AccessDenied(
                "شما اجازه ثبت وظیفه برای این یگان را ندارید.");
        }

        ViewBag.DiningHalls = unitQuotaManager
            .GetDiningHalls(
                model.OrgId,
                model.PersonalTypeId)
            .ToList();

        return PartialView("_Duty", model);
    }

    #endregion

    #region ثبت وظیفه

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateDuty(
        UnitQuotaDutyCreateDTO model)
    {
        try
        {
            if (!CanView())
                return AccessDenied();

            if (model == null)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "اطلاعات ارسالی معتبر نیست."
                });
            }

            if (!ModelState.IsValid)
                return ModelStateError();

            var result =
                unitQuotaManager.CreateDuty(
                    model);

            return Json(result);
        }
        catch
        {
            return Json(new
            {
                Status = false,
                Message =
                    "ثبت کارکنان وظیفه با خطا همراه بوده است."
            });
        }
    }

    #endregion

    #region دریافت نفرات

    [HttpGet]
    public IActionResult GetPersons(
        long unitQuotaId,
        long mealId)
    {
        if (!CanView())
            return AccessDenied();

        var persons =
            unitQuotaManager.GetPersons(
                unitQuotaId,
                mealId);

        return Json(new
        {
            Status = true,
            Model = persons
        });
    }

    #endregion

    #region حذف فرد

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePerson(long id)
    {
        try
        {
            if (!CanView())
                return AccessDenied();

            if (id <= 0)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "شناسه فرد معتبر نیست."
                });
            }

            var result =
                unitQuotaManager.DeletePerson(id);

            return Json(result);
        }
        catch
        {
            return Json(new
            {
                Status = false,
                Message =
                    "حذف فرد با خطا همراه بوده است."
            });
        }
    }

    #endregion
}