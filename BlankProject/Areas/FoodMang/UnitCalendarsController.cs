using Azure.Core;
using BLL;
using BLL.Interface;
using Domain.Enums;
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
        private const string RegistrarRole =
            "ثبت‌کننده آمار یگان";

        private const string ApproverRole =
            "تایید کننده آمار یگان";

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

            //accessToken =
            //    apiTokenClient.GetApiToken(
            //        CustomSettings.Instance.ClientId,
            //        CustomSettings.Instance.Scope,
            //        CustomSettings.Instance.ClientSecret,
            //        CustomSettings.Instance.ROPC_UserName,
            //        CustomSettings.Instance.ROPC_Password
            //    ).Result;

            accessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjkxRUQ1RDFGMEIxQzg3ODQ3NzE4QjMyNEQwQkM5QkU5IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3ODU0MDEyNDgsImV4cCI6MTc4NTQwNDg0OCwiaXNzIjoiaHR0cDovL2l0b2lkZW50aXR5c2VydmVyLm5lei5uZXQiLCJhdWQiOlsiT3JnYW5BcGkiLCJQZXJzb25lbEFwaSIsIlByb3ZpbmNlQXBpIl0sImNsaWVudF9pZCI6IkRlcHJpdmF0aW9uIiwic3ViIjoiZGVwcml2YXRpb24iLCJhdXRoX3RpbWUiOjE3ODU0MDEyNDgsImlkcCI6ImxvY2FsIiwianRpIjoiMUJCM0FEQzIwNjg0NDYxNDNFOTkyMzM3M0RENUJGMkUiLCJpYXQiOjE3ODU0MDEyNDgsInNjb3BlIjpbIm9yZ2FuLmluZm8iLCJwZXJzb25hbC5pbmZvIiwicHJvdmluY2UuaW5mbyJdLCJhbXIiOlsiY3VzdG9tIl19.GcBFFzII5Q66M35Rr2Mk6_FWPE-YihRJco5TDz3q91vjOvO4_KpemZDtQsX3o9SeplTTls-mjEeLWxmkBD6f56fnsGyGNkDFSK5yPZ_C3B83413r_E1s2mOu8yU7yeDznMH5sagFRH5BQX32Kw3tk2mO-vgIXdscr2VvQZmnDPfw_K0Z9HSiJt6VAEN_9jdYsrZoInvjAyDBSSYvdSTQGjHCAbcSGmMNgScfUB4IjlwD1xhMMmt_WiNHupRI7QwXZ4WGRiN1oNtrU-1T7GnlKukVL1kR4_V3uR_ZmT5UwZnSCbtq8hxv9tOK3UqAEM5kucGxYtldeCj8jmWXnoXp7g";

        }

        private UserSessionDTO GetCurrentUser()
        {
            return Session?.GetUser();
        }

        private static string NormalizeRole(
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value
                .Trim()
                .Replace("\u200c", "")
                .Replace(" ", "")
                .Replace("ي", "ی")
                .Replace("ك", "ک");
        }

        private bool HasRole(
            string roleTitle)
        {
            var user =
                GetCurrentUser();

            if (user == null ||
                !user.IsEnabled)
            {
                return false;
            }

            return string.Equals(
                NormalizeRole(user.Role),
                NormalizeRole(roleTitle),
                StringComparison.OrdinalIgnoreCase);
        }

        private bool CanManage()
        {
            return
                HasRole(RegistrarRole) ||
                HasRole(ApproverRole);
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

            if (!ModelState.IsValid)
                return ModelStateError();

            var user =
                GetCurrentUser();

            if (user == null ||
                user.OmdOrgId <= 0)
            {
                return AccessDenied(
                    "یگان کاربر مشخص نشده است.");
            }

            var orgTitle =
                GetOrgTitle(
                    user.OmdOrgId);

            var result =
                unitCalendarManager.Create(
                    model,
                    user.OmdOrgId,
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
                user.OmdOrgId <= 0)
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
                user.OmdOrgId <= 0)
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