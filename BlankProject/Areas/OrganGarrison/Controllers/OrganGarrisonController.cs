using BLL;
using BLL.Garrision;
using BLL.Interface;
using DTO;
using DTO.Entities.Garrison;
using Filters;
using ITOWebApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.RedisService;
using Services.SessionServices;

namespace Food.Areas.OrganGarrison.Controllers
{
    /// <summary>
    /// مدیریت یگان‌های پادگان
    /// </summary>
    [Area("OrganGarrison")]
    [UserAuthorize(Area: "OrganGarrison", Controller: "OrganGarrison", Action: "index")]
    public class OrganGarrisonController : Controller
    {
        private readonly ISession Session;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IDataTableManager dataTableManager;
        private readonly IRedisManager Redis;
        private readonly ApiTokenCacheClient apiTokenClient;
        private readonly IWebApiManager webApiManager;
        private readonly IOrganGarrisionManager organGarrisionManager;
        private readonly IPersonalManager personalManager;
        private readonly IPersonManager personManager;

        private string access_token = string.Empty;

        public OrganGarrisonController(
            IHttpContextAccessor _httpContextAccessor,
            IDataTableManager _dataTableManager,
            IRedisManager _Redis,
            IWebApiManager _webApiManager,
            ApiTokenCacheClient _apiTokenClient,
            IOrganGarrisionManager _organGarrisionManager,
            IPersonalManager _personalManager,
            IPersonManager _personManager)
        {
            httpContextAccessor = _httpContextAccessor;
            dataTableManager = _dataTableManager;
            Redis = _Redis;
            webApiManager = _webApiManager;
            apiTokenClient = _apiTokenClient;
            organGarrisionManager = _organGarrisionManager;
            personalManager = _personalManager;
            personManager = _personManager;

            Session = httpContextAccessor.HttpContext.Session;
            access_token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjkxRUQ1RDFGMEIxQzg3ODQ3NzE4QjMyNEQwQkM5QkU5IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3ODU0MDEyNDgsImV4cCI6MTc4NTQwNDg0OCwiaXNzIjoiaHR0cDovL2l0b2lkZW50aXR5c2VydmVyLm5lei5uZXQiLCJhdWQiOlsiT3JnYW5BcGkiLCJQZXJzb25lbEFwaSIsIlByb3ZpbmNlQXBpIl0sImNsaWVudF9pZCI6IkRlcHJpdmF0aW9uIiwic3ViIjoiZGVwcml2YXRpb24iLCJhdXRoX3RpbWUiOjE3ODU0MDEyNDgsImlkcCI6ImxvY2FsIiwianRpIjoiMUJCM0FEQzIwNjg0NDYxNDNFOTkyMzM3M0RENUJGMkUiLCJpYXQiOjE3ODU0MDEyNDgsInNjb3BlIjpbIm9yZ2FuLmluZm8iLCJwZXJzb25hbC5pbmZvIiwicHJvdmluY2UuaW5mbyJdLCJhbXIiOlsiY3VzdG9tIl19.GcBFFzII5Q66M35Rr2Mk6_FWPE-YihRJco5TDz3q91vjOvO4_KpemZDtQsX3o9SeplTTls-mjEeLWxmkBD6f56fnsGyGNkDFSK5yPZ_C3B83413r_E1s2mOu8yU7yeDznMH5sagFRH5BQX32Kw3tk2mO-vgIXdscr2VvQZmnDPfw_K0Z9HSiJt6VAEN_9jdYsrZoInvjAyDBSSYvdSTQGjHCAbcSGmMNgScfUB4IjlwD1xhMMmt_WiNHupRI7QwXZ4WGRiN1oNtrU-1T7GnlKukVL1kR4_V3uR_ZmT5UwZnSCbtq8hxv9tOK3UqAEM5kucGxYtldeCj8jmWXnoXp7g";

            //access_token = apiTokenClient.GetApiToken(
            //    CustomSettings.Instance.ClientId,
            //    CustomSettings.Instance.Scope,
            //    CustomSettings.Instance.ClientSecret,
            //    CustomSettings.Instance.ROPC_UserName,
            //    CustomSettings.Instance.ROPC_Password
            //).Result;
        }

        #region نمایش

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetList(OrganGarrisonDTO filters)
        {
            var searchModel = dataTableManager.GetSearchModel();
            var model = organGarrisionManager.GetDataTableDTO(searchModel, filters);

            return Json(model);
        }

        #endregion

        #region ایجاد یگان

        public IActionResult LoadCreateForm()
        {
            FillOrganGarrisonViewData(null);

            var model = new OrganGarrisonDTO();

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrganGarrisonDTO model)
        {
            try
            {
                var user = Session.GetUser();

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

                if (model.OrgId == null || model.OrgId <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "انتخاب یگان الزامی است"
                    });
                }

                model.Title = webApiManager.GetUnitDutyByUnitId(model.OrgId.Value, access_token);

                var res = organGarrisionManager.Create(model, user.Id);

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

        #region ویرایش یگان

        public IActionResult LoadEditForm(long id)
        {
            var model = organGarrisionManager.LoadEditForm(id);

            if (model == null)
                return NotFound();

            FillOrganGarrisonViewData(model);

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrganGarrisonDTO model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "یگان مورد نظر یافت نشد!"
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

                if (model.OrgId == null || model.OrgId <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "انتخاب یگان الزامی است"
                    });
                }

                model.Title = webApiManager.GetUnitDutyByUnitId(model.OrgId.Value, access_token);

                var res = organGarrisionManager.Update(model);

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

        #region پرسنل یگان

        public IActionResult LoadCreatePersons(long id)
        {
            ViewData["PersonType"] = new SelectList(personalManager.GetSelectListDTO(), "Id", "Title");
            ViewData["OrganGarrisonId"] = id;

            var model = new PersonDTO
            {
                OrganGarrisonId = id
            };

            return PartialView("_AddPersons", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPerson(PersonDTO model)
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
                        Status = false,
                        Message = string.Join("</br>", errors)
                    });
                }

                if (model.OrganGarrisonId == null || model.OrganGarrisonId <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "شناسه یگان نامعتبر است"
                    });
                }

                var isExist = personManager.ExistPerson(model);
                if (isExist)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "پرسنل مورد نظر قبلاً ثبت شده است!"
                    });
                }

                var res = personManager.Create(model);
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

        [HttpPost]
        public IActionResult GetListPerson(long? id, PersonDTO filters)
        {
            var searchModel = dataTableManager.GetSearchModel();
            var model = personManager.GetDataTableDTO(searchModel, filters, id);

            return Json(model);
        }

        public IActionResult OnGetGetPersonalId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json(null);

            var result = webApiManager.GetPersonalByPersonCode(id, access_token);

            return Json(result);
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
                        Message = "پرسنل مورد نظر یافت نشد!"
                    });
                }

                var person = personManager.GetById(id);
                if (person == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "پرسنل مورد نظر یافت نشد!"
                    });
                }

                var res = personManager.Delete(person);

                return Json(new
                {
                    Status = res,
                    Message = res ? "پرسنل با موفقیت حذف شد" : "حذف پرسنل با خطا همراه بوده است"
                });
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "حذف پرسنل با خطا همراه بوده است!"
                });
            }
        }

        #endregion

        #region Helper

        private void FillOrganGarrisonViewData(OrganGarrisonDTO model)
        {
            ViewData["GarsionType"] = new SelectList(
                organGarrisionManager.GetorganGarrisonTypes(),
                "Id",
                "Title",
                model?.OrganGarrisonTypeId);

            ViewData["OrgId"] = new SelectList(
                webApiManager.GetOrganByCategoryCode(access_token),
                "Id",
                "UnitTitle",
                model?.OrgId);

            ViewData["ParentId"] = new SelectList(
                organGarrisionManager.GetParent(),
                "Id",
                "Title",
                model?.ParentId);
        }

        #endregion
    }
}