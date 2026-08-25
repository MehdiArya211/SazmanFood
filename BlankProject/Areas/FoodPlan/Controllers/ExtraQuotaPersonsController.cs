using BLL.Interface;
using Domain.Enums;
using DTO.Entities;
using DTO.User;
using Filters;
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
        private const string SupportRole =
            "رکن 4 پشتیبانی قرارگاه";

        private const string RegistrarRole =
            "ثبت‌کننده آمار یگان";

        private const string ApproverRole =
            "تایید کننده آمار یگان";

        private const string OfficeRole =
            "اداری یگان";

        private readonly IExtraQuotaPersonManager manager;
        private readonly IUnitQuotaManager unitQuotaManager;
        private readonly ISession Session;

        public ExtraQuotaPersonsController(
            IExtraQuotaPersonManager manager,
            IUnitQuotaManager unitQuotaManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.manager = manager;
            this.unitQuotaManager = unitQuotaManager;
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

            if (user == null || !user.IsEnabled)
                return false;

            var role = user.Role?.Trim();

            return role == SupportRole ||
                   role == RegistrarRole ||
                   role == ApproverRole ||
                   role == OfficeRole;
        }

        /// <summary>
        /// تعیین و حذف پرسنل فقط توسط اداری یگان
        /// یا رکن چهار پشتیبانی مجاز است.
        /// </summary>
        private bool CanManagePersons()
        {
            var user = GetCurrentUser();

            if (user == null || !user.IsEnabled)
                return false;

            var role = user.Role?.Trim();

            return string.Equals(
                       role,
                       OfficeRole,
                       StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(
                       role,
                       SupportRole,
                       StringComparison.OrdinalIgnoreCase);
        }

        private bool CanViewAllOrganizations()
        {
            var user = GetCurrentUser();

            return user != null &&
                   user.IsEnabled &&
                   user.Role?.Trim() == SupportRole;
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