using Azure.Core;
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

namespace Food.Areas.FoodMang.Controllers
{
    [Area("FoodMang")]
    [UserAuthorize(
        Area: "FoodMang",
        Controller: "UnitCalendars",
        Action: "index")]
    public class UnitCalendarsController
        : Controller
    {
        private readonly IUnitCalendarManager
            unitCalendarManager;

        private readonly IWebApiManager
            webApiManager;

        private readonly ISession Session;

        private readonly ApiTokenCacheClient
            apiTokenClient;

        private string accessToken =
            string.Empty;

        public UnitCalendarsController(
            IHttpContextAccessor httpContextAccessor,
            IUnitCalendarManager unitCalendarManager,
            IWebApiManager webApiManager,
            ApiTokenCacheClient apiTokenClient)
        {
            this.unitCalendarManager =
                unitCalendarManager ??
                throw new ArgumentNullException(
                    nameof(unitCalendarManager));

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

            accessToken = apiTokenClient.GetApiToken(
                CustomSettings.Instance.ClientId,
                CustomSettings.Instance.Scope,
                CustomSettings.Instance.ClientSecret,
                CustomSettings.Instance.ROPC_UserName,
                CustomSettings.Instance.ROPC_Password
            ).GetAwaiter().GetResult();

        }

        private UserSessionDTO GetCurrentUser()
        {
            return Session?.GetUser();
        }

        private bool CanManage()
        {
            var user = GetCurrentUser();
            return user?.IsEnabled == true &&
                   (user.RoleId == RoleConstant.Admin ||
                    user.RoleId == RoleConstant.FoodRegistrar ||
                    user.RoleId == RoleConstant.FoodApprover);
        }

        private IActionResult AccessDenied(
            string message =
                "شما مجوز مدیریت تقویم یگان را ندارید.")
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

        private string GetOrgTitle(
            int orgId)
        {
            if (orgId <= 0)
                return null;

            return webApiManager
                .GetListOrganInfoV1(
                    accessToken)
                .Where(x =>
                    x.Id == orgId)
                .Select(x =>
                    x.UnitTitle)
                .FirstOrDefault();
        }

        private void LoadDayTypes(
            DayType? selected = null)
        {
            var items =
                new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value =
                            ((int)DayType.Normal)
                                .ToString(),
                        Text = "عادی",
                        Selected =
                            selected ==
                            DayType.Normal
                    },

                    new SelectListItem
                    {
                        Value =
                            ((int)DayType.Holiday)
                                .ToString(),
                        Text = "تعطیل",
                        Selected =
                            selected ==
                            DayType.Holiday
                    },

                    new SelectListItem
                    {
                        Value =
                            ((int)DayType.HalfHoliday)
                                .ToString(),
                        Text = "نیمه تعطیل",
                        Selected =
                            selected ==
                            DayType.HalfHoliday
                    }
                };

            ViewData["DayTypes"] =
                items;
        }

        public IActionResult Index()
        {
            if (!CanManage())
                return AccessDenied();

            LoadDayTypes();

            return View();
        }

        [HttpPost]
        public IActionResult GetList(
            UnitCalendarFilterDTO filters)
        {
            if (!CanManage())
                return AccessDenied();

            var result =
                unitCalendarManager.GetList(
                    filters);

            return Json(new
            {
                data = result
            });
        }

        public IActionResult LoadCreateForm()
        {
            if (!CanManage())
                return AccessDenied();

            LoadDayTypes(
                DayType.Normal);
            if (GetCurrentUser()?.RoleId == RoleConstant.Admin)
                ViewBag.Organizations = new SelectList(
                    webApiManager.GetListOrganInfoV1(accessToken), "Id", "UnitTitle");

            return PartialView(
                "_Create",
                new UnitCalendarCreateDTO
                {
                    CalendarDate =
                        DateTime.Now.Date,

                    DayType =
                        DayType.Normal
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            UnitCalendarCreateDTO model)
        {
            if (!CanManage())
                return AccessDenied();

            if (model == null)
                return BadRequest(new { Status = false, Message = "اطلاعات ارسالی معتبر نیست." });

            if (!ModelState.IsValid)
                return ModelStateError();

            var user =
                GetCurrentUser();

            if (user == null ||
                (user.RoleId != RoleConstant.Admin && user.OmdOrgId <= 0))
            {
                return AccessDenied(
                    "یگان کاربر مشخص نشده است.");
            }

            var orgId = user.RoleId == RoleConstant.Admin
                ? model.OrgId.GetValueOrDefault()
                : user.OmdOrgId;

            if (orgId <= 0)
                return Json(new { Status = false, Message = "یگان را انتخاب کنید." });

            var orgTitle = GetOrgTitle(orgId);

            var result =
                unitCalendarManager.Create(
                    model,
                    orgId,
                    orgTitle,
                    user.Id);

            return Json(result);
        }

        public IActionResult LoadEditForm(
            long id)
        {
            if (!CanManage())
                return AccessDenied();

            var model =
                unitCalendarManager.GetEditDTO(
                    id);

            if (model == null)
                return NotFound();

            LoadDayTypes(
                model.DayType);

            return PartialView(
                "_Edit",
                model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            UnitCalendarEditDTO model)
        {
            if (!CanManage())
                return AccessDenied();

            if (!ModelState.IsValid)
                return ModelStateError();

            var user =
                GetCurrentUser();

            if (user == null ||
                (user.RoleId != RoleConstant.Admin && user.OmdOrgId <= 0))
            {
                return AccessDenied(
                    "یگان کاربر مشخص نشده است.");
            }

            var result =
                unitCalendarManager.Update(
                    model,
                    user.OmdOrgId,
                    user.Id);

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(
            long id)
        {
            if (!CanManage())
                return AccessDenied();

            var user =
                GetCurrentUser();

            if (user == null ||
                (user.RoleId != RoleConstant.Admin && user.OmdOrgId <= 0))
            {
                return AccessDenied(
                    "یگان کاربر مشخص نشده است.");
            }

            var result =
                unitCalendarManager.Delete(
                    id,
                    user.OmdOrgId,
                    user.Id);

            return Json(result);
        }
    }
}