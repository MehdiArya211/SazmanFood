using BLL;
using BLL.Interface;
using Domain.Enums;
using Domain.Constants;
using DTO.Entities;
using DTO.User;
using Filters;
using ITOWebApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.SessionServices;

namespace Food.Areas.FoodPlan.Controllers
{
    [Area("FoodPlan")]
    [UserAuthorize(
        Area: "FoodPlan",
        Controller: "ExtraQuotaPersons",
        Action: "Index")]
    public class ExtraQuotaPersonsController : Controller
    {
        private readonly IExtraQuotaPersonManager manager;
        private readonly IUnitQuotaManager unitQuotaManager;
        private readonly IWebApiManager webApiManager;
        private readonly ApiTokenCacheClient apiTokenCacheClient;
        private readonly ISession Session;

        public ExtraQuotaPersonsController(
            IExtraQuotaPersonManager manager,
            IUnitQuotaManager unitQuotaManager,
            IWebApiManager webApiManager,
            ApiTokenCacheClient apiTokenCacheClient,
            IHttpContextAccessor httpContextAccessor)
        {
            this.manager = manager;
            this.unitQuotaManager = unitQuotaManager;
            this.webApiManager = webApiManager;
            this.apiTokenCacheClient = apiTokenCacheClient;
            Session = httpContextAccessor.HttpContext.Session;
        }

        public IActionResult Index()
        {
            if (!CanView())
                return AccessDenied();

            ViewBag.CanViewAllOrganizations =
                CanViewAllOrganizations();

            ViewBag.CanManagePersons =
                CanManagePersons();

            return View();
        }

        [HttpPost]
        public IActionResult GetList(int? orgId)
        {
            if (!CanView())
                return AccessDenied();

            var user = GetCurrentUser();

            if (!CanViewAllOrganizations())
                orgId = user.OmdOrgId;

            var data =
                manager.GetList(
                    CanViewAllOrganizations(),
                    orgId);

            return Json(new
            {
                data = data.Select((x, index) => new
                {
                    row = index + 1,
                    requestId = x.RequestId,
                    orgTitle = x.OrgTitle,
                    mealTitle = x.MealTitle,
                    fromDateFa = x.FromDateFa,
                    toDateFa = x.ToDateFa,
                    count = x.Count,
                    registeredCount = x.RegisteredCount,
                    personalTypeId = x.PersonalTypeId,
                    personalTypeTitle = x.PersonalTypeTitle
                })
            });
        }

        public IActionResult LoadPersonsForm(long id)
        {
            if (!CanView())
                return AccessDenied();

            var model =
                manager.GetForm(
                    id,
                    CanViewAllOrganizations());

            if (model == null)
                return NotFound();

            ViewBag.CanManagePersons =
                CanManagePersons();

            ViewBag.DiningHalls =
                unitQuotaManager.GetDiningHalls(
                    model.OrgId,
                    model.PersonalTypeId);

            return PartialView("_Persons", model);
        }

        /// <summary>
        /// دریافت اطلاعات پرسنل کادر براساس کد پرسنلی
        /// </summary>
        [HttpGet]
        public IActionResult GetOfficialPerson(string personCode)
        {
            if (!CanManagePersons())
                return AccessDenied("شما مجوز جستجوی پرسنل را ندارید.");

            personCode = personCode?.Trim();

            if (string.IsNullOrWhiteSpace(personCode))
            {
                return Json(new
                {
                    Status = false,
                    Message = "کد پرسنلی الزامی است."
                });
            }

            try
            {
                var token = apiTokenCacheClient.GetApiToken(
                    CustomSettings.Instance.ClientId,
                    CustomSettings.Instance.Scope,
                    CustomSettings.Instance.ClientSecret,
                    CustomSettings.Instance.ROPC_UserName,
                    CustomSettings.Instance.ROPC_Password
                ).GetAwaiter().GetResult();

                var person =
                    webApiManager.GetPersonalByPersonCode(
                        personCode,
                        token);

                if (person == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "پرسنل مورد نظر یافت نشد."
                    });
                }

                return Json(new
                {
                    Status = true,
                    Model = new
                    {
                        Id = person.Id,
                        PersonCode = person.personalCode,
                        NationalCode = person.MelliCode,
                        RankTitle = person.RankTitle,
                        FullName = !string.IsNullOrWhiteSpace(person.FullName)
                            ? person.FullName
                            : (person.FirstName + " " + person.LastName).Trim()
                    }
                });
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "دریافت اطلاعات پرسنل با خطا همراه بود."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOfficial(
            ExtraQuotaOfficialPersonCreateDTO model)
        {
            if (!CanManagePersons())
                return AccessDenied("شما مجوز تعیین پرسنل بن مازاد را ندارید.");

            if (!ModelState.IsValid)
                return ModelStateError();

            var result =
                manager.CreateOfficial(
                    model,
                    CanViewAllOrganizations());

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDuty(
            ExtraQuotaDutyPersonCreateDTO model)
        {
            if (!CanManagePersons())
                return AccessDenied("شما مجوز تعیین پرسنل بن مازاد را ندارید.");

            if (!ModelState.IsValid)
                return ModelStateError();

            var result =
                manager.CreateDuty(
                    model,
                    CanViewAllOrganizations());

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePerson(long id)
        {
            if (!CanManagePersons())
                return AccessDenied("شما مجوز حذف پرسنل بن مازاد را ندارید.");

            var result =
                manager.DeletePerson(
                    id,
                    CanViewAllOrganizations());

            return Json(result);
        }

        private UserSessionDTO GetCurrentUser()
        {
            return Session?.GetUser();
        }

        private bool CanView()
        {
            var user = GetCurrentUser();
            return user?.IsEnabled == true &&
                   (user.RoleId == RoleConstant.Admin ||
                    user.RoleId == RoleConstant.FoodSupport ||
                    user.RoleId == RoleConstant.FoodRegistrar ||
                    user.RoleId == RoleConstant.FoodApprover ||
                    user.RoleId == RoleConstant.FoodOffice);
        }

        /// <summary>تعیین نفرات برای اداری یگان و پشتیبانی قرارگاه مجاز است.</summary>
        private bool CanManagePersons()
        {
            var user = GetCurrentUser();
            return user?.IsEnabled == true &&
                   (user.RoleId == RoleConstant.Admin ||
                    user.RoleId == RoleConstant.FoodOffice ||
                    user.RoleId == RoleConstant.FoodSupport);
        }

        private bool CanViewAllOrganizations()
        {
            var user = GetCurrentUser();
            return user?.IsEnabled == true && (user.RoleId == RoleConstant.Admin || user.RoleId == RoleConstant.FoodSupport);
        }

        private IActionResult AccessDenied(
            string message = "شما مجوز انجام این عملیات را ندارید.")
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Status = false,
                    Message = message
                });
        }

        private JsonResult ModelStateError()
        {
            var errors =
                ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct();

            return Json(new
            {
                Status = false,
                Message = string.Join("</br>", errors)
            });
        }
    }
}