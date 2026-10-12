using BLL;
using BLL.Interface;
using ClosedXML.Excel;
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
            //if (user.RoleId != RoleConstant.Admin && person.UnitCode !=
            //    user.OmdOrgId)
            //{
            //    return Json(new
            //    {
            //        Status = false,
            //        Message =
            //            "این پرسنل متعلق به یگان شما نیست."
            //    });
            //}

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

    #region ثبت گروهی با Excel

    /// <summary>
    /// دانلود فایل نمونه ثبت گروهی کارکنان.
    /// </summary>
    [HttpGet]
    public IActionResult DownloadExcelTemplate(string personnelType)
    {
        personnelType =
            personnelType?
                .Trim()
                .ToLowerInvariant();

        if (personnelType != "official" &&
            personnelType != "duty")
        {
            return BadRequest(
                "نوع پرسنل معتبر نیست.");
        }

        using var workbook =
            new XLWorkbook();

        var worksheet =
            workbook.Worksheets.Add(
                personnelType == "official"
                    ? "کارکنان کادر"
                    : "کارکنان وظیفه");

        worksheet.RightToLeft = true;

        if (personnelType == "official")
        {
            worksheet.Cell(1, 1).Value =
                "کد پرسنلی";

            worksheet.Cell(2, 1).Value =
                "12345678";

            worksheet.Column(1).Width = 25;
        }
        else
        {
            worksheet.Cell(1, 1).Value =
                "کد ملی";

            worksheet.Cell(1, 2).Value =
                "کد پرسنلی";

            worksheet.Cell(1, 3).Value =
                "نام و نشان";

            worksheet.Cell(2, 1).Value =
                "0012345678";

            worksheet.Cell(2, 2).Value =
                "12345678";

            worksheet.Cell(2, 3).Value =
                "نام نمونه";

            worksheet.Column(1).Width = 18;
            worksheet.Column(2).Width = 22;
            worksheet.Column(3).Width = 35;
        }

        var lastColumn =
            personnelType == "official"
                ? 1
                : 3;

        var header =
            worksheet.Range(
                1,
                1,
                1,
                lastColumn);

        header.Style.Font.Bold = true;
        header.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
        header.Style.Fill.BackgroundColor =
            XLColor.LightBlue;

        worksheet.SheetView.FreezeRows(1);

        using var stream =
            new MemoryStream();

        workbook.SaveAs(stream);

        var fileName =
            personnelType == "official"
                ? "UnitQuota-Official-Template.xlsx"
                : "UnitQuota-Duty-Template.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>
    /// ثبت گروهی کارکنان از فایل Excel با استفاده از همان قواعد ثبت تکی.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ImportExcel(
        IFormFile file,
        long unitQuotaId,
        long mealId,
        long diningHallId,
        string personnelType)
    {
        try
        {
            if (!CanView())
                return AccessDenied();

            personnelType =
                personnelType?
                    .Trim()
                    .ToLowerInvariant();

            if (personnelType != "official" &&
                personnelType != "duty")
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "نوع پرسنل معتبر نیست."
                });
            }

            if (file == null ||
                file.Length <= 0)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "فایل Excel را انتخاب کنید."
                });
            }

            if (!string.Equals(
                    Path.GetExtension(file.FileName),
                    ".xlsx",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "فقط فایل Excel با پسوند xlsx قابل قبول است."
                });
            }

            if (file.Length >
                10 * 1024 * 1024)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "حجم فایل نباید بیشتر از 10 مگابایت باشد."
                });
            }

            if (unitQuotaId <= 0 ||
                mealId <= 0 ||
                diningHallId <= 0)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "سهمیه، وعده یا سالن غذاخوری معتبر نیست."
                });
            }

            var quotaForm =
                personnelType == "official"
                    ? unitQuotaManager.GetOfficialForm(
                        unitQuotaId,
                        mealId)
                    : unitQuotaManager.GetDutyForm(
                        unitQuotaId,
                        mealId);

            if (quotaForm == null)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "سهمیه فعال و قابل ثبت یافت نشد."
                });
            }

            if (!CanManageOrganization(
                    quotaForm.OrgId))
            {
                return AccessDenied(
                    "شما اجازه ثبت پرسنل برای این یگان را ندارید.");
            }

            using var workbook =
                new XLWorkbook(
                    file.OpenReadStream());

            var worksheet =
                workbook.Worksheets
                    .FirstOrDefault();

            if (worksheet == null)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "فایل Excel فاقد شیت قابل پردازش است."
                });
            }

            var rows =
                worksheet.RowsUsed()
                    .Skip(1)
                    .Where(x =>
                        x.CellsUsed().Any(c =>
                            !string.IsNullOrWhiteSpace(
                                c.GetFormattedString())))
                    .ToList();

            if (!rows.Any())
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "فایل Excel فاقد اطلاعات پرسنل است."
                });
            }

            if (rows.Count > 2000)
            {
                return Json(new
                {
                    Status = false,
                    Message =
                        "حداکثر 2000 نفر در هر فایل قابل ثبت است."
                });
            }

            var results =
                new List<object>();

            var inserted = 0;
            var duplicate = 0;
            var failed = 0;

            var seen =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                var rowNumber =
                    row.RowNumber();

                if (personnelType == "official")
                {
                    var personCode =
                        row.Cell(1)
                            .GetFormattedString()?
                            .Trim()
                            .ToEnglishNumber();

                    if (string.IsNullOrWhiteSpace(
                            personCode))
                    {
                        failed++;

                        results.Add(new
                        {
                            Row = rowNumber,
                            PersonCode = "",
                            FullName = "",
                            Status = "error",
                            Message =
                                "کد پرسنلی وارد نشده است."
                        });

                        continue;
                    }

                    if (!seen.Add(personCode))
                    {
                        duplicate++;

                        results.Add(new
                        {
                            Row = rowNumber,
                            PersonCode = personCode,
                            FullName = "",
                            Status = "duplicate",
                            Message =
                                "این کد پرسنلی در فایل تکرار شده است."
                        });

                        continue;
                    }

                    var person =
                        webApiManager
                            .GetPersonalByPersonCode(
                                personCode,
                                access_token);

                    if (person == null)
                    {
                        failed++;

                        results.Add(new
                        {
                            Row = rowNumber,
                            PersonCode = personCode,
                            FullName = "",
                            Status = "error",
                            Message =
                                "پرسنل در سرویس پرسنلی یافت نشد."
                        });

                        continue;
                    }

                    var model =
                        new UnitQuotaOfficialCreateDTO
                        {
                            UnitQuotaId =
                                unitQuotaId,

                            MealId =
                                mealId,

                            PersonCode =
                                personCode,

                            DiningHallId =
                                diningHallId
                        };

                    var saveResult =
                        unitQuotaManager
                            .CreateOfficial(
                                model,
                                person);

                    var fullName =
                        !string.IsNullOrWhiteSpace(
                            person.FullName)
                            ? person.FullName.Trim()
                            : $"{person.FirstName} {person.LastName}"
                                .Trim();

                    var message =
                        saveResult?.Message ??
                        "ثبت انجام نشد.";

                    var isSuccess =
                        saveResult?.Status == true;

                    var isDuplicate =
                        !isSuccess &&
                        (message.Contains("قبلاً") ||
                         message.Contains("تکرار") ||
                         message.Contains("وجود دارد"));

                    if (isSuccess)
                        inserted++;
                    else if (isDuplicate)
                        duplicate++;
                    else
                        failed++;

                    results.Add(new
                    {
                        Row = rowNumber,
                        PersonCode = personCode,
                        FullName = fullName,
                        Status =
                            isSuccess
                                ? "success"
                                : isDuplicate
                                    ? "duplicate"
                                    : "error",
                        Message = message
                    });
                }
                else
                {
                    var nationalCode =
                        row.Cell(1)
                            .GetFormattedString()?
                            .Trim()
                            .ToEnglishNumber();

                    var personCode =
                        row.Cell(2)
                            .GetFormattedString()?
                            .Trim()
                            .ToEnglishNumber();

                    var fullName =
                        row.Cell(3)
                            .GetFormattedString()?
                            .Trim();

                    if (string.IsNullOrWhiteSpace(
                            nationalCode) ||
                        nationalCode.Length != 10 ||
                        !nationalCode.All(char.IsDigit))
                    {
                        failed++;

                        results.Add(new
                        {
                            Row = rowNumber,
                            PersonCode = personCode ?? "",
                            FullName = fullName ?? "",
                            Status = "error",
                            Message =
                                "کد ملی باید 10 رقم باشد."
                        });

                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(
                            personCode) ||
                        string.IsNullOrWhiteSpace(
                            fullName))
                    {
                        failed++;

                        results.Add(new
                        {
                            Row = rowNumber,
                            PersonCode = personCode ?? "",
                            FullName = fullName ?? "",
                            Status = "error",
                            Message =
                                "کد پرسنلی و نام و نشان الزامی است."
                        });

                        continue;
                    }

                    var key =
                        nationalCode +
                        "|" +
                        personCode;

                    if (!seen.Add(key))
                    {
                        duplicate++;

                        results.Add(new
                        {
                            Row = rowNumber,
                            PersonCode = personCode,
                            FullName = fullName,
                            Status = "duplicate",
                            Message =
                                "این فرد در فایل تکرار شده است."
                        });

                        continue;
                    }

                    var model =
                        new UnitQuotaDutyCreateDTO
                        {
                            UnitQuotaId =
                                unitQuotaId,

                            MealId =
                                mealId,

                            NationalCode =
                                nationalCode,

                            PersonCode =
                                personCode,

                            FullName =
                                fullName,

                            DiningHallId =
                                diningHallId
                        };

                    var saveResult =
                        unitQuotaManager
                            .CreateDuty(model);

                    var message =
                        saveResult?.Message ??
                        "ثبت انجام نشد.";

                    var isSuccess =
                        saveResult?.Status == true;

                    var isDuplicate =
                        !isSuccess &&
                        (message.Contains("قبلاً") ||
                         message.Contains("تکرار") ||
                         message.Contains("وجود دارد"));

                    if (isSuccess)
                        inserted++;
                    else if (isDuplicate)
                        duplicate++;
                    else
                        failed++;

                    results.Add(new
                    {
                        Row = rowNumber,
                        PersonCode = personCode,
                        FullName = fullName,
                        Status =
                            isSuccess
                                ? "success"
                                : isDuplicate
                                    ? "duplicate"
                                    : "error",
                        Message = message
                    });
                }
            }

            return Json(new
            {
                Status = inserted > 0,
                Message =
                    inserted > 0
                        ? $"{inserted} نفر با موفقیت ثبت شد."
                        : "رکورد جدیدی ثبت نشد.",
                Model = new
                {
                    Total =
                        results.Count,

                    Inserted =
                        inserted,

                    Duplicate =
                        duplicate,

                    Failed =
                        failed,

                    Rows =
                        results
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                Status = false,
                Message =
                    "پردازش فایل Excel با خطا همراه بود. " +
                    ex.Message
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