using BLL;
using BLL.Interface;
using Domain.Enums;
using Domain.Constants;
using DTO.Entities;
using DTO.User;
using Filters;
using ITOWebApiClient;
using Microsoft.AspNetCore.Mvc;
using Services.SessionServices;

namespace Food.Areas.FoodMang.Controllers
{
    /// <summary>
    /// مدیریت آمار یگان
    /// </summary>
    [Area("FoodMang")]
    [UserAuthorize(
        Area: "FoodMang",
        Controller: "UnitStatistics",
        Action: "index")]
    public class UnitStatisticsController : Controller
    {
        private readonly IUnitStatisticManager unitStatisticManager;
        private readonly IDataTableManager dataTableManager;
        private readonly IWebApiManager webApiManager;
        private readonly ISession Session;
        private readonly ApiTokenCacheClient apiTokenClient;
        private string access_token = string.Empty;

        public UnitStatisticsController(
            IHttpContextAccessor httpContextAccessor,
            IUnitStatisticManager unitStatisticManager,
            IDataTableManager dataTableManager,
            IWebApiManager webApiManager,
            ApiTokenCacheClient apiTokenClient)
        {
            this.unitStatisticManager =
                unitStatisticManager ??
                throw new ArgumentNullException(nameof(unitStatisticManager));

            this.dataTableManager =
                dataTableManager ??
                throw new ArgumentNullException(nameof(dataTableManager));

            this.webApiManager =
                webApiManager ??
                throw new ArgumentNullException(nameof(webApiManager));

            this.apiTokenClient =
                apiTokenClient ??
                throw new ArgumentNullException(nameof(apiTokenClient));

            Session = httpContextAccessor?.HttpContext?.Session;
            access_token = apiTokenClient.GetApiToken(
                CustomSettings.Instance.ClientId,
                CustomSettings.Instance.Scope,
                CustomSettings.Instance.ClientSecret,
                CustomSettings.Instance.ROPC_UserName,
                CustomSettings.Instance.ROPC_Password
            ).GetAwaiter().GetResult();
        }

        #region دسترسی‌ها

        private UserSessionDTO GetCurrentUser()
        {
            return Session?.GetUser();
        }

        private bool IsRegistrar()
        {
            var user = GetCurrentUser();
            return user?.IsEnabled == true && (user.RoleId == RoleConstant.Admin || user.RoleId == RoleConstant.FoodRegistrar);
        }

        private bool IsApprover()
        {
            var user = GetCurrentUser();
            return user?.IsEnabled == true && (user.RoleId == RoleConstant.Admin || user.RoleId == RoleConstant.FoodApprover);
        }

        private bool CanView()
        {
            return IsRegistrar() || IsApprover();
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

        private IActionResult ModelStateError()
        {
            var errors = ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage);

            return Json(new
            {
                Status = false,
                Message = string.Join("</br>", errors)
            });
        }

        private string GetOrgTitle(int orgId)
        {
            if (orgId <= 0)
                return null;

            return webApiManager
                .GetListOrganInfoV1(access_token)
                .Where(x => x.Id == orgId)
                .Select(x => x.UnitTitle)
                .FirstOrDefault();
        }

        #endregion

        #region نمایش

        public IActionResult Index()
        {
            if (!CanView())
                return AccessDenied();

            ViewBag.CanRegister = IsRegistrar();
            ViewBag.CanApprove = IsApprover();

            return View();
        }

        [HttpPost]
        public IActionResult GetList(UnitStatisticFilterDTO filters)
        {
            if (!CanView())
                return AccessDenied();

            var user = GetCurrentUser();

            if (user == null)
                return AccessDenied("اطلاعات کاربر در سشن یافت نشد.");

            if (user.RoleId != RoleConstant.Admin && user.OmdOrgId <= 0)
                return AccessDenied("یگان کاربر مشخص نشده است.");

            filters ??= new UnitStatisticFilterDTO();

            /*
             * کاربر فقط آمار یگان خودش را مشاهده می‌کند.
             * OrgId ارسال‌شده از مرورگر نادیده گرفته می‌شود.
             */
            if (user.RoleId != RoleConstant.Admin)
                filters.OrgId = user.OmdOrgId;

            var searchModel = dataTableManager.GetSearchModel();

            var result = unitStatisticManager.GetDataTableDTO(
                searchModel,
                filters);

            return Json(result);
        }

        #endregion

        #region ثبت اولیه

        public IActionResult LoadCreateForm()
        {
            if (!IsRegistrar())
            {
                return AccessDenied(
                    "فقط ثبت‌کننده آمار اجازه ثبت آمار جدید را دارد.");
            }

            var user = GetCurrentUser();

            if (user == null)
                return AccessDenied("اطلاعات کاربر در سشن یافت نشد.");

            if (user.RoleId != RoleConstant.Admin && user.OmdOrgId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "یگان کاربر مشخص نشده است."
                });
            }

            if (user.RoleId == RoleConstant.Admin)
            {
                ViewBag.Organizations = new SelectList(
                    webApiManager.GetListOrganInfoV1(access_token),
                    "Id", "UnitTitle");
                return PartialView("_Create", new UnitStatisticCreateDTO());
            }

            var orgTitle = GetOrgTitle(user.OmdOrgId);

            if (string.IsNullOrWhiteSpace(orgTitle))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "عنوان یگان کاربر یافت نشد."
                });
            }

            var model = new UnitStatisticCreateDTO
            {
                OrgId = user.OmdOrgId,
                OrgTitle = orgTitle
            };

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UnitStatisticCreateDTO model)
        {
            try
            {
                if (!IsRegistrar())
                {
                    return AccessDenied(
                        "فقط ثبت‌کننده آمار اجازه ثبت آمار جدید را دارد.");
                }

                var user = GetCurrentUser();

                if (user == null)
                    return AccessDenied("اطلاعات کاربر در سشن یافت نشد.");

                if (user.RoleId != RoleConstant.Admin && user.OmdOrgId <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "یگان کاربر مشخص نشده است."
                    });
                }

                if (model == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "اطلاعات ارسالی معتبر نیست."
                    });
                }

                /*
                 * شناسه و عنوان یگان از سشن و وب‌سرویس گرفته می‌شود؛
                 * مقادیر ارسال‌شده از مرورگر قابل اعتماد نیست.
                 */
                if (user.RoleId != RoleConstant.Admin)
                    model.OrgId = user.OmdOrgId;
                model.OrgTitle = GetOrgTitle(model.OrgId);

                ModelState.Remove(nameof(model.OrgId));
                ModelState.Remove(nameof(model.OrgTitle));

                if (string.IsNullOrWhiteSpace(model.OrgTitle))
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "عنوان یگان کاربر یافت نشد."
                    });
                }

                if (!ModelState.IsValid)
                    return ModelStateError();

                var result = unitStatisticManager.Create(
                    model,
                    user.Id,
                    user.FullName);

                return Json(result);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ثبت آمار یگان با خطا همراه بوده است."
                });
            }
        }

        #endregion

        #region ویرایش

        public IActionResult LoadEditForm(long id)
        {
            if (!IsRegistrar())
            {
                return AccessDenied(
                    "فقط ثبت‌کننده آمار اجازه ویرایش آمار را دارد.");
            }

            var user = GetCurrentUser();

            if (user == null)
                return AccessDenied("اطلاعات کاربر در سشن یافت نشد.");

            if (id <= 0)
                return NotFound();

            var model = unitStatisticManager.GetEditDTO(id);

            if (model == null)
                return NotFound();

            if (user.RoleId != RoleConstant.Admin && model.OrgId != user.OmdOrgId)
            {
                return AccessDenied(
                    "شما اجازه ویرایش آمار این یگان را ندارید.");
            }

            if (model.Status == UnitStatisticStatus.Approved ||
                model.Status == UnitStatisticStatus.Canceled)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = model.Status == UnitStatisticStatus.Canceled
                        ? "آمار لغوشده قابل ویرایش نیست."
                        : "آمار تأیید نهایی شده قابل ویرایش نیست."
                });
            }

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UnitStatisticEditDTO model)
        {
            try
            {
                if (!IsRegistrar())
                {
                    return AccessDenied(
                        "فقط ثبت‌کننده آمار اجازه ویرایش آمار را دارد.");
                }

                var user = GetCurrentUser();

                if (user == null)
                    return AccessDenied("اطلاعات کاربر در سشن یافت نشد.");

                if (model == null || model.Id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "آمار مورد نظر یافت نشد."
                    });
                }

                var current = unitStatisticManager.GetEditDTO(model.Id);

                if (current == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "آمار مورد نظر یافت نشد."
                    });
                }

                if (user.RoleId != RoleConstant.Admin && current.OrgId != user.OmdOrgId)
                {
                    return AccessDenied(
                        "شما اجازه ویرایش آمار این یگان را ندارید.");
                }

                if (current.Status == UnitStatisticStatus.Approved ||
                    current.Status == UnitStatisticStatus.Canceled)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = current.Status == UnitStatisticStatus.Canceled
                            ? "آمار لغوشده قابل ویرایش نیست."
                            : "آمار تأیید نهایی شده قابل ویرایش نیست."
                    });
                }

                /*
                 * جلوگیری از تغییر یگان و وضعیت از طریق دست‌کاری فرم.
                 */
                model.OrgId = current.OrgId;
                model.OrgTitle = current.OrgTitle;
                model.Status = current.Status;

                ModelState.Remove(nameof(model.OrgId));
                ModelState.Remove(nameof(model.OrgTitle));
                ModelState.Remove(nameof(model.Status));

                if (!ModelState.IsValid)
                    return ModelStateError();

                var result = unitStatisticManager.Update(model);

                return Json(result);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ویرایش آمار یگان با خطا همراه بوده است."
                });
            }
        }

        #endregion

        #region جزئیات

        public IActionResult LoadDetailsForm(long id)
        {
            if (!CanView())
                return AccessDenied();

            var user = GetCurrentUser();

            if (user == null)
                return AccessDenied("اطلاعات کاربر در سشن یافت نشد.");

            if (id <= 0)
                return NotFound();

            var model = unitStatisticManager.GetDetailsDTO(id);

            if (model == null)
                return NotFound();

            if (user.RoleId != RoleConstant.Admin && model.OrgId != user.OmdOrgId)
            {
                return AccessDenied(
                    "شما اجازه مشاهده آمار این یگان را ندارید.");
            }

            ViewBag.CanRegister =
                IsRegistrar() &&
                model.Status != UnitStatisticStatus.Approved &&
                model.Status != UnitStatisticStatus.Canceled;

            ViewBag.CanApprove =
                IsApprover() &&
                model.Status == UnitStatisticStatus.Sent;

            return PartialView("_Details", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveDetails(UnitStatisticDetailsDTO model)
        {
            try
            {
                if (!IsRegistrar())
                {
                    return AccessDenied(
                        "فقط ثبت‌کننده آمار اجازه ثبت جزئیات را دارد.");
                }

                var user = GetCurrentUser();

                if (user == null)
                    return AccessDenied("اطلاعات کاربر در سشن یافت نشد.");

                if (user.RoleId != RoleConstant.Admin && user.OmdOrgId <= 0)
                    return AccessDenied("یگان کاربر مشخص نشده است.");

                if (model == null || model.Id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "اطلاعات جزئیات معتبر نیست."
                    });
                }

                var current = unitStatisticManager.GetDetailsDTO(model.Id);

                if (current == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "آمار مورد نظر یافت نشد."
                    });
                }

                if (user.RoleId != RoleConstant.Admin && current.OrgId != user.OmdOrgId)
                {
                    return AccessDenied(
                        "شما اجازه ثبت جزئیات این یگان را ندارید.");
                }

                if (current.Status == UnitStatisticStatus.Approved)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "جزئیات آمار تأییدشده قابل ویرایش نیست."
                    });
                }

                /*
                 * اطلاعات سربرگ از دیتابیس دریافت می‌شود؛
                 * فقط لیست جزئیات از فرم پذیرفته می‌شود.
                 */
                model.OrgId = current.OrgId;
                model.OrgTitle = current.OrgTitle;
                model.TotalOfficialCount = current.TotalOfficialCount;
                model.TotalDutyCount = current.TotalDutyCount;
                model.Status = current.Status;

                ModelState.Remove(nameof(model.OrgId));
                ModelState.Remove(nameof(model.OrgTitle));
                ModelState.Remove(nameof(model.TotalOfficialCount));
                ModelState.Remove(nameof(model.TotalDutyCount));
                ModelState.Remove(nameof(model.Status));

                if (!ModelState.IsValid)
                    return ModelStateError();

                var officialSum =
                    model.OfficialDetails?.Sum(x => x.Count) ?? 0;

                var dutySum =
                    model.DutyDetails?.Sum(x => x.Count) ?? 0;

                if (officialSum != current.TotalOfficialCount)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            $"جمع تعداد کادر واردشده ({officialSum}) " +
                            $"با تعداد کل کادر ({current.TotalOfficialCount}) " +
                            "همخوانی ندارد."
                    });
                }

                if (dutySum != current.TotalDutyCount)
                {
                    return Json(new
                    {
                        Status = false,
                        Message =
                            $"جمع تعداد وظیفه واردشده ({dutySum}) " +
                            $"با تعداد کل وظیفه ({current.TotalDutyCount}) " +
                            "همخوانی ندارد."
                    });
                }

                var result = unitStatisticManager.SaveDetails(model);

                return Json(result);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ذخیره جزئیات آمار با خطا همراه بوده است."
                });
            }
        }

        #endregion

        #region ارسال

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Send(long id)
        {
            try
            {
                if (!IsRegistrar())
                {
                    return AccessDenied(
                        "فقط ثبت‌کننده آمار اجازه ارسال آمار را دارد.");
                }

                var user = GetCurrentUser();

                if (user == null)
                    return AccessDenied("اطلاعات کاربر در سشن یافت نشد.");

                if (user.RoleId != RoleConstant.Admin && user.OmdOrgId <= 0)
                    return AccessDenied("یگان کاربر مشخص نشده است.");

                if (id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "شناسه آمار معتبر نیست."
                    });
                }

                var current = unitStatisticManager.GetDetailsDTO(id);

                if (current == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "آمار مورد نظر یافت نشد."
                    });
                }

                if (user.RoleId != RoleConstant.Admin && current.OrgId != user.OmdOrgId)
                {
                    return AccessDenied(
                        "شما اجازه ارسال آمار این یگان را ندارید.");
                }

                if (current.Status != UnitStatisticStatus.Draft)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "فقط آمار ثبت اولیه قابل ارسال است."
                    });
                }

                var result = unitStatisticManager.Send(id);

                return Json(result);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ارسال آمار با خطا همراه بوده است."
                });
            }
        }

        #endregion

        #region تأیید نهایی

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(long id)
        {
            try
            {
                if (!IsApprover())
                {
                    return AccessDenied(
                        "فقط تأییدکننده آمار اجازه تأیید نهایی را دارد.");
                }

                var user = GetCurrentUser();

                if (user == null)
                    return AccessDenied("اطلاعات کاربر در سشن یافت نشد.");

                if (user.RoleId != RoleConstant.Admin && user.OmdOrgId <= 0)
                    return AccessDenied("یگان کاربر مشخص نشده است.");

                if (id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "شناسه آمار معتبر نیست."
                    });
                }

                var current = unitStatisticManager.GetDetailsDTO(id);

                if (current == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "آمار مورد نظر یافت نشد."
                    });
                }

                if (user.RoleId != RoleConstant.Admin && current.OrgId != user.OmdOrgId)
                {
                    return AccessDenied(
                        "شما اجازه تأیید آمار این یگان را ندارید.");
                }

                if (current.Status != UnitStatisticStatus.Sent)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "فقط آمار ارسال‌شده قابل تأیید نهایی است."
                    });
                }

                var result = unitStatisticManager.Approve(id);

                return Json(result);
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "تأیید نهایی آمار و محاسبه سهمیه با خطا همراه بوده است."
                });
            }
        }

        #endregion

        #region عودت

        /// <summary>
        /// عودت آمار ارسال‌شده برای اصلاح
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Return(long id, string reason)
        {
            try
            {
                if (!IsApprover())
                    return AccessDenied("فقط تأییدکننده آمار اجازه عودت را دارد.");

                if (id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "شناسه آمار معتبر نیست."
                    });
                }

                if (string.IsNullOrWhiteSpace(reason))
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "ثبت دلیل عودت الزامی است."
                    });
                }

                var current = unitStatisticManager.GetDetailsDTO(id);

                if (current == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "آمار مورد نظر یافت نشد."
                    });
                }

                if (current.Status != UnitStatisticStatus.Sent)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "فقط آمار ارسال‌شده قابل عودت است."
                    });
                }

                return Json(unitStatisticManager.Return(id, reason));
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "عودت آمار با خطا همراه بوده است."
                });
            }
        }

        #endregion

        #region لغو

        /// <summary>
        /// لغو آمار ارسال‌شده
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(long id, string reason)
        {
            try
            {
                if (!IsApprover())
                    return AccessDenied("فقط تأییدکننده آمار اجازه لغو را دارد.");

                if (id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "شناسه آمار معتبر نیست."
                    });
                }

                if (string.IsNullOrWhiteSpace(reason))
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "ثبت دلیل لغو الزامی است."
                    });
                }

                var current = unitStatisticManager.GetDetailsDTO(id);

                if (current == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "آمار مورد نظر یافت نشد."
                    });
                }

                if (current.Status != UnitStatisticStatus.Sent)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "فقط آمار ارسال‌شده قابل لغو است."
                    });
                }

                return Json(unitStatisticManager.Cancel(id, reason));
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "لغو آمار با خطا همراه بوده است."
                });
            }
        }

        #endregion

    }
}