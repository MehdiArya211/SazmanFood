using BLL;
using ClosedXML.Excel;
using Domain.Constants;
using BLL.Interface;
using Domain.Enums;
using DTO.Base;
using DTO.User;
using FajrLog.Enum;
using Filters;
using ITOWebApiClient;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Services.RedisService;
using Services.SessionServices;
using Utilities.Extentions;

namespace Food.Areas.AuthSystem.Controllers
{
    /// <summary>
    /// مدیریت کاربران - پرسنل
    /// </summary>
    [Area("AuthSystem")]
    [UserAuthorize(Area: "AuthSystem", Controller: "users", Action: "index")]
    public class UsersController : Controller
    {
        private readonly IUserManager UserManager;
        private readonly IRoleManager roleManager;
        private readonly IUserPasswordHistoryManager PasswordHistoryManager;
        private readonly IDataTableManager dataTableManager;
        private readonly IRedisManager Redis;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IWebApiManager webApiManager;
        private readonly ISession Session;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IRedisManager _Redis,
            IHttpContextAccessor _httpContextAccessor,
            IUserManager _UserManager,
            IRoleManager _roleManager,
            IUserPasswordHistoryManager _PasswordHistoryManager,
            IDataTableManager _dataTableManager,
            ApiTokenCacheClient apiTokenCache,
            IWebApiManager _webApiManager,
            IServiceScopeFactory scopeFactory,
            ILogger<UsersController> logger)
        {
            UserManager = _UserManager;
            roleManager = _roleManager;
            dataTableManager = _dataTableManager;
            Redis = _Redis;
            _apiTokenClient = apiTokenCache;
            webApiManager = _webApiManager;
            Session = _httpContextAccessor.HttpContext.Session;

            _scopeFactory = scopeFactory;
            _logger = logger;

            PasswordHistoryManager = _PasswordHistoryManager;
        }

        private string GetAccessToken() => _apiTokenClient.GetApiToken(
            CustomSettings.Instance.ClientId,
            CustomSettings.Instance.Scope,
            CustomSettings.Instance.ClientSecret,
            CustomSettings.Instance.ROPC_UserName,
            CustomSettings.Instance.ROPC_Password).GetAwaiter().GetResult();

        private bool CanImportMealBookers() =>
            Session.GetUser()?.RoleId == RoleConstant.Admin;

        private sealed record MealBookerImportRowResult(
            int Row, string PersonCode, string FullName, string Status, string Message);

        #region نمایش همه

        public IActionResult Index()
        {
            ViewData["Roles"] = new SelectList(
                roleManager.GetSelectListDTO(),
                "Id",
                "Title"
            );

            return View();
        }

        [HttpPost]
        public ActionResult GetList(UserFilterDataTableDTO filters)
        {
            var SearchModel = dataTableManager.GetSearchModel();

            var model = UserManager.GetDataTableDTO(
                SearchModel,
                filters
            );

            return Json(model);
        }

        #endregion

        #region ثبت گروهی کاربران رزرو غذا

        [HttpGet]
        public IActionResult ImportMealBookers()
        {
            if (!CanImportMealBookers())
                return Forbid();

            return View();
        }

        [HttpGet]
        public IActionResult DownloadMealBookersTemplate()
        {
            if (!CanImportMealBookers())
                return Forbid();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("کاربران رزرو غذا");
            sheet.RightToLeft = true;
            sheet.Cell(1, 1).Value = "کد پرسنلی";
            sheet.Cell(1, 2).Value = "نام";
            sheet.Cell(1, 3).Value = "نام خانوادگی";
            sheet.Column(1).Style.NumberFormat.Format = "@";
            sheet.Column(1).Width = 23;
            sheet.Column(2).Width = 25;
            sheet.Column(3).Width = 30;
            var header = sheet.Range(1, 1, 1, 3);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightGreen;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "MealBookersTemplate.xlsx");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImportMealBookers(IFormFile file)
        {
            if (!CanImportMealBookers())
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { status = false, message = "دسترسی به ثبت گروهی کاربران ندارید." });

            if (file == null || file.Length == 0)
                return Json(new { status = false, message = "فایل اکسل را انتخاب کنید." });

            if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx",
                    StringComparison.OrdinalIgnoreCase) ||
                file.Length > 5 * 1024 * 1024)
                return Json(new { status = false,
                    message = "فقط فایل xlsx با حجم حداکثر ۵ مگابایت پذیرفته می‌شود." });

            try
            {
                using var fileStream = file.OpenReadStream();
                using var workbook = new XLWorkbook(fileStream);
                var sheet = workbook.Worksheets.FirstOrDefault();
                if (sheet == null || sheet.FirstRowUsed() == null)
                    return Json(new { status = false, message = "فایل اکسل فاقد اطلاعات است." });

                var headerRow = sheet.FirstRowUsed();
                if (headerRow.RowNumber() != 1 ||
                    headerRow.Cell(1).HasFormula || headerRow.Cell(2).HasFormula ||
                    headerRow.Cell(3).HasFormula)
                    return Json(new { status = false,
                        message = "عنوان ستون‌ها باید در ردیف اول و بدون فرمول باشد." });

                var codeHeader = NormalizeImportHeader(headerRow.Cell(1).GetString());
                var nameHeader = NormalizeImportHeader(headerRow.Cell(2).GetString());
                var familyHeader = NormalizeImportHeader(headerRow.Cell(3).GetString());
                var combinedName = nameHeader is "نامونشان" or "نامکامل" or
                    "نامونامخانوادگی" or "fullname";
                var splitName = (nameHeader == "نام" || nameHeader == "firstname") &&
                    (familyHeader == "نامخانوادگی" || familyHeader == "lastname" ||
                     familyHeader == "familyname");

                if (codeHeader is not ("کدپرسنلی" or "personcode" or "personnelcode") ||
                    (!combinedName && !splitName))
                    return Json(new { status = false,
                        message = "ستون‌ها باید «کد پرسنلی، نام، نام خانوادگی» یا «کد پرسنلی، نام و نشان» باشند. فایل نمونه را دریافت کنید." });

                var rows = sheet.RowsUsed().Skip(1)
                    .Where(row => Enumerable.Range(1, combinedName ? 2 : 3)
                        .Any(column => !row.Cell(column).IsEmpty()))
                    .Take(1001).ToList();
                if (rows.Count == 0)
                    return Json(new { status = false, message = "فایل اکسل ردیف پرسنلی ندارد." });
                if (rows.Count > 1000)
                    return Json(new { status = false, message = "در هر فایل حداکثر ۱۰۰۰ نفر قابل ثبت است." });

                var results = new List<MealBookerImportRowResult>();
                var seen = new HashSet<string>(StringComparer.Ordinal);
                var created = 0;
                var existing = 0;
                var failed = 0;
                var creatorId = Session.GetUser().Id;

                foreach (var row in rows)
                {
                    var rowNumber = row.RowNumber();
                    var personCode = "";
                    var fullName = "";
                    try
                    {
                        if (Enumerable.Range(1, combinedName ? 2 : 3)
                            .Any(column => row.Cell(column).HasFormula))
                            throw new ArgumentException("استفاده از فرمول در ستون‌های اطلاعات پرسنل مجاز نیست.");

                        personCode = row.Cell(1).GetFormattedString()?.Trim().ToEnglishNumber() ?? "";
                        var firstOrFullName = row.Cell(2).GetFormattedString()?.Trim() ?? "";
                        var lastName = combinedName ? "" : row.Cell(3).GetFormattedString()?.Trim() ?? "";
                        fullName = combinedName
                            ? firstOrFullName
                            : $"{firstOrFullName} {lastName}".Trim();

                        if (personCode.Length < 6 || personCode.Length > 19 ||
                            personCode.Any(c => c < '0' || c > '9'))
                            throw new ArgumentException("کد پرسنلی باید ۶ تا ۱۹ رقم باشد. ستون کد را در اکسل از نوع Text قرار دهید.");
                        if (string.IsNullOrWhiteSpace(firstOrFullName) ||
                            (!combinedName && string.IsNullOrWhiteSpace(lastName)))
                            throw new ArgumentException("نام و نام خانوادگی هر دو الزامی هستند.");
                        if (fullName.Length > 50)
                            throw new ArgumentException("نام کامل نباید بیشتر از ۵۰ کاراکتر باشد.");
                        if (!seen.Add(personCode))
                            throw new ArgumentException("این کد پرسنلی در همین فایل تکرار شده است.");

                        // Each row gets a fresh DbContext: a failed SaveChanges must
                        // not leave tracked entities behind for the following row.
                        using var scope = _scopeFactory.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                        int? numericCode = int.TryParse(personCode, out var parsedCode)
                            ? parsedCode : null;
                        var conflictingAccount = context.Users.FirstOrDefault(x =>
                            x.IsDeleted == false &&
                            numericCode.HasValue && x.PersonCode == numericCode &&
                            x.Username != personCode);
                        if (conflictingAccount != null)
                        {
                            failed++;
                            results.Add(new MealBookerImportRowResult(
                                rowNumber, personCode, fullName, "failed",
                                "این کد پرسنلی به حسابی با نام کاربری متفاوت متصل است؛ با مدیر سامانه بررسی کنید."));
                            continue;
                        }

                        var provisioning = scope.ServiceProvider
                            .GetRequiredService<IReservationUserProvisioningManager>();
                        var saved = provisioning.EnsureReservationUser(
                            null, personCode, fullName, null, null, creatorId);

                        if (!saved.Status)
                        {
                            failed++;
                            results.Add(new MealBookerImportRowResult(
                                rowNumber, personCode, fullName, "failed",
                                saved.Message ?? "ثبت حساب انجام نشد."));
                            continue;
                        }

                        var alreadyExists = saved.Message?.Contains("قبلاً", StringComparison.Ordinal) == true;
                        if (alreadyExists) existing++;
                        else created++;
                        results.Add(new MealBookerImportRowResult(
                            rowNumber, personCode, fullName,
                            alreadyExists ? "existing" : "created",
                            alreadyExists
                                ? "حساب از قبل وجود دارد؛ رمز و دسترسی‌های قبلی حفظ شد."
                                : "حساب رزروکننده ایجاد شد."));
                    }
                    catch (ArgumentException ex)
                    {
                        failed++;
                        results.Add(new MealBookerImportRowResult(
                            rowNumber, personCode, fullName, "failed", ex.Message));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "ثبت گروهی کاربر رزرو غذا در ردیف {Row} ناموفق بود.", rowNumber);
                        failed++;
                        results.Add(new MealBookerImportRowResult(
                            rowNumber, personCode, fullName, "failed",
                            "ثبت حساب این ردیف با خطای سیستمی مواجه شد."));
                    }
                }

                return Json(new
                {
                    status = true,
                    total = results.Count,
                    created,
                    existing,
                    failed,
                    rows = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "خواندن فایل ثبت گروهی کاربران رزرو غذا ناموفق بود.");
                return Json(new { status = false,
                    message = "فایل اکسل خوانده نشد. فایل نمونه با پسوند xlsx را بررسی کنید." });
            }
        }

        private static string NormalizeImportHeader(string value) =>
            new string((value ?? "").Trim()
                .Replace('ي', 'ی').Replace('ك', 'ک')
                .Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

        #endregion

        #region ایجاد

        public IActionResult LoadCreateForm()
        {
            ViewData["RoleId"] = new SelectList(
                roleManager.GetSelectListDTO(),
                "Id",
                "Title"
            );

            ViewData["UserType"] = new SelectList(
                EnumExtensions.ToEnumViewModel<UserType>(),
                "Id",
                "Title"
            );

            ViewData["Gharargah"] = new SelectList(
                webApiManager.GetOrganInfo(GetAccessToken()),
                "Id",
                "UnitTitle"
            );


            return PartialView("_Create", new UserCreateDTO());
        }


        //[HttpGet]
        //public IActionResult GetOrgByGharargah(int id)
        //{
        //    var result = webApiManager.GetOrganInfo(
        //        id,
        //        access_token
        //    );

        //    return Json(result);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserCreateDTO model)
        {
            try
            {
                if (model.RoleId == 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "نوع دسترسی کاربر را مشخص کنید!"
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

                var res = UserManager.Create(model);

                PasswordHistoryManager.CheckPasswordHistory(
                    res.Model,
                    model.Password,
                    false
                );

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


        #endregion


        #region ویرایش

        public IActionResult LoadEditForm(long id)
        {
            var user = UserManager.GetEditDTO(id);

            if (user == null)
                return NotFound();

            ViewData["RoleId"] = new SelectList(
                roleManager.GetSelectListDTO(),
                "Id",
                "Title",
                user.RoleId
            );

            ViewData["UserType"] = new SelectList(
                EnumExtensions.ToEnumViewModel<UserType>(),
                "Id",
                "Title",
                user.Type
            );

            ViewData["Gharargah"] = new SelectList(
                webApiManager.GetOrganInfo(GetAccessToken()),
                "Id",
                "UnitTitle",
                user.OmdOrgId
            );


            return PartialView("_Edit", user);
        }


        [HttpGet]
        public IActionResult GetEditOrgByGharargah(int id)
        {
            var result = webApiManager.GetOrganByGharargahId(
                id,
                GetAccessToken()
            );

            return Json(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, UserEditDTO model)
        {
            try
            {
                if (id != model.Id)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "کاربر یافت نشد!"
                    });
                }

                if (model.RoleId == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "نوع دسترسی کاربر را مشخص کنید!"
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

                return Json(UserManager.Update(model));
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

        #endregion


        #region تغییر رمز

        public IActionResult LoadChangePasswordForm(long id, string FullName)
        {
            ViewBag.FullName = FullName;

            return PartialView(
                "_ChangePassword",
                new UserChangePasswordDTO
                {
                    Id = id
                }
            );
        }


        [HttpPost]
        public IActionResult ChangePassword(long id, UserChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Status = false,
                    Message = "اطلاعات صحیح نیست"
                });
            }

            return Json(
                UserManager.ChangePassword(model)
            );
        }

        #endregion


        #region فعال غیر فعال

        public IActionResult ToggleEnable(long id)
        {
            var model = UserManager.GetById(id);

            model.IsEnabled = !model.IsEnabled;

            var res = UserManager.Update(model);

            return Json(new
            {
                res.Status,
                model.IsEnabled
            });
        }

        #endregion


        #region حذف

        [HttpPost]
        public IActionResult Delete(long id)
        {
            var result = UserManager.Delete(id);

            return Json(new
            {
                Status = result,
                Message = result
                    ? "کاربر حذف شد"
                    : "حذف انجام نشد"
            });
        }

        #endregion
    }
}