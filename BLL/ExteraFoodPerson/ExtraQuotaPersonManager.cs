using BLL.Interface;
using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using DTO.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Services.SessionServices;

namespace BLL
{
    public class ExtraQuotaPersonManager
        : Manager<GuestExtraFoodRequestPerson, ApplicationContext>,
          IExtraQuotaPersonManager
    {
        private readonly ISession Session;

        public ExtraQuotaPersonManager(
            DbContexts contexts,
            IHttpContextAccessor httpContextAccessor)
            : base(contexts, httpContextAccessor)
        {
            Session =
                httpContextAccessor.HttpContext?.Session;
        }

        public List<ExtraQuotaPersonListDTO> GetList(
            bool canViewAll,
            int? orgId)
        {
            var user =
                Session?.GetUser();

            if (user == null)
                return new();

            if (!canViewAll)
                orgId = user.OmdOrgId;

            var requests =
                UOW.GuestExtraFoodRequest
                    .GetAll()
                    .Where(x =>
                        x.IsDeleted != true &&
                        x.Status ==
                            GuestExtraFoodRequestStatus.Approved &&
                        x.ExtraQuotaCount > 0);

            if (orgId.HasValue && orgId.Value > 0)
            {
                requests = requests.Where(x =>
                    x.OrgId == orgId.Value);
            }

            var meals =
                UOW.Meal.GetAll().ToList();

            var personalTypes =
                UOW.PersonalType.GetAll().ToList();

            var persons =
                UOW.GuestExtraFoodRequestPerson
                    .GetAll()
                    .Where(x =>
                        x.IsDeleted != true)
                    .ToList();

            return requests
                .OrderByDescending(x => x.Id)
                .Select(x => new ExtraQuotaPersonListDTO
                {
                    RequestId = x.Id,
                    OrgId = x.OrgId,
                    OrgTitle = x.OrgTitle,
                    MealId = x.MealId,
                    MealTitle =
                        meals.FirstOrDefault(m =>
                            m.Id == x.MealId)?.Title,
                    PersonalTypeId = x.PersonalTypeId,
                    PersonalTypeTitle =
                        personalTypes.FirstOrDefault(p =>
                            p.Id == x.PersonalTypeId)?.Title,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    Count = x.ExtraQuotaCount,
                    RegisteredCount =
                        persons.Count(p =>
                            p.GuestExtraFoodRequestId == x.Id)
                })
                .ToList();
        }

        public ExtraQuotaPersonFormDTO GetForm(
            long requestId,
            bool canViewAll)
        {
            var request =
                GetAuthorizedRequest(
                    requestId,
                    canViewAll);

            if (request == null)
                return null;

            if (request.Status !=
                GuestExtraFoodRequestStatus.Approved)
            {
                return null;
            }

            if (request.ExtraQuotaCount <= 0)
                return null;

            var meal =
                UOW.Meal.FirstOrDefault(x =>
                    x.Id == request.MealId);

            var personalType =
                UOW.PersonalType.FirstOrDefault(x =>
                    x.Id == request.PersonalTypeId);

            var persons =
                UOW.GuestExtraFoodRequestPerson
                    .GetByRequestId(requestId)
                    .Select(x => new ExtraQuotaPersonDTO
                    {
                        Id = x.Id,
                        RequestId =
                            x.GuestExtraFoodRequestId,
                        RankTitle = x.RankTitle,
                        FullName = x.FullName,
                        PersonCode = x.PersonCode,
                        NationalCode = x.NationalCode,
                        DiningHallId = x.DiningHallId,
                        DiningHallTitle =
                            x.DiningHall?.Title
                    })
                    .ToList();

            var personalTypeCode =
                personalType?.Code ?? 0;

            return new ExtraQuotaPersonFormDTO
            {
                RequestId = request.Id,
                OrgId = request.OrgId,
                OrgTitle = request.OrgTitle,
                MealId = request.MealId,
                MealTitle = meal?.Title,
                PersonalTypeId = request.PersonalTypeId,
                PersonalTypeTitle = personalType?.Title,
                Count = request.ExtraQuotaCount,
                RegisteredCount = persons.Count,
                IsOfficial =
                    personalTypeCode ==
                    PersonalTypeCodes.Official,
                IsDuty =
                    personalTypeCode ==
                    PersonalTypeCodes.Duty,
                Persons = persons
            };
        }

        public BaseResult CreateOfficial(
            ExtraQuotaOfficialPersonCreateDTO model,
            bool canViewAll)
        {
            var user =
                Session?.GetUser();

            if (user == null)
                return new(false, "اطلاعات کاربر یافت نشد.");

            if (model == null)
                return new(false, "اطلاعات ارسالی معتبر نیست.");

            var request =
                GetAuthorizedRequest(
                    model.RequestId,
                    canViewAll);

            if (request == null)
                return new(false, "درخواست یافت نشد.");

            var validation =
                ValidateRequestForCreate(request);

            if (!validation.Status)
                return validation;

            if (!IsOfficial(request.PersonalTypeId))
            {
                return new(
                    false,
                    "نوع پرسنل این درخواست کادر نیست.");
            }

            if (string.IsNullOrWhiteSpace(model.PersonCode))
                return new(false, "کد پرسنلی الزامی است.");

            if (string.IsNullOrWhiteSpace(model.FullName))
                return new(false, "نام و نشان الزامی است.");

            if (!model.DiningHallId.HasValue ||
                model.DiningHallId.Value <= 0)
            {
                return new(false, "سالن غذاخوری الزامی است.");
            }

            if (UOW.GuestExtraFoodRequestPerson.HasPersonCode(
                    request.Id,
                    model.PersonCode))
            {
                return new(
                    false,
                    "این کد پرسنلی قبلاً برای این درخواست ثبت شده است.");
            }

            var entity =
                new GuestExtraFoodRequestPerson
                {
                    GuestExtraFoodRequestId = request.Id,
                    OrgId = request.OrgId,
                    MealId = request.MealId,
                    PersonalTypeId = request.PersonalTypeId,
                    DiningHallId = model.DiningHallId,
                    PersonId = model.PersonId,
                    PersonCode =
                        model.PersonCode?.Trim(),
                    NationalCode =
                        model.NationalCode?.Trim(),
                    RankTitle =
                        model.RankTitle?.Trim(),
                    FullName =
                        model.FullName?.Trim(),
                    CreatorId = user.Id,
                    CreatorFullName = user.FullName,
                    RegUserId = user.Id,
                    RegDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    IsDeleted = false
                };

            UOW.GuestExtraFoodRequestPerson.Add(entity);

            var success =
                UOW.Commit();

            return new(
                success,
                success
                    ? "پرسنل با موفقیت ثبت شد."
                    : "ثبت پرسنل با خطا همراه بود.");
        }

        public BaseResult CreateDuty(
            ExtraQuotaDutyPersonCreateDTO model,
            bool canViewAll)
        {
            var user =
                Session?.GetUser();

            if (user == null)
                return new(false, "اطلاعات کاربر یافت نشد.");

            if (model == null)
                return new(false, "اطلاعات ارسالی معتبر نیست.");

            var request =
                GetAuthorizedRequest(
                    model.RequestId,
                    canViewAll);

            if (request == null)
                return new(false, "درخواست یافت نشد.");

            var validation =
                ValidateRequestForCreate(request);

            if (!validation.Status)
                return validation;

            if (!IsDuty(request.PersonalTypeId))
            {
                return new(
                    false,
                    "نوع پرسنل این درخواست وظیفه نیست.");
            }

            if (string.IsNullOrWhiteSpace(model.NationalCode))
                return new(false, "کد ملی الزامی است.");

            if (model.NationalCode.Trim().Length != 10)
                return new(false, "کد ملی باید 10 رقم باشد.");

            if (string.IsNullOrWhiteSpace(model.PersonCode))
                return new(false, "شماره پرسنلی الزامی است.");

            if (string.IsNullOrWhiteSpace(model.FullName))
                return new(false, "نام و نشان الزامی است.");

            if (UOW.GuestExtraFoodRequestPerson.HasNationalCode(
                    request.Id,
                    model.NationalCode))
            {
                return new(
                    false,
                    "این کد ملی قبلاً برای این درخواست ثبت شده است.");
            }

            if (UOW.GuestExtraFoodRequestPerson.HasPersonCode(
                    request.Id,
                    model.PersonCode))
            {
                return new(
                    false,
                    "این شماره پرسنلی قبلاً برای این درخواست ثبت شده است.");
            }

            var entity =
                new GuestExtraFoodRequestPerson
                {
                    GuestExtraFoodRequestId = request.Id,
                    OrgId = request.OrgId,
                    MealId = request.MealId,
                    PersonalTypeId = request.PersonalTypeId,
                    PersonCode =
                        model.PersonCode?.Trim(),
                    NationalCode =
                        model.NationalCode?.Trim(),
                    FullName =
                        model.FullName?.Trim(),
                    CreatorId = user.Id,
                    CreatorFullName = user.FullName,
                    RegUserId = user.Id,
                    RegDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    IsDeleted = false
                };

            UOW.GuestExtraFoodRequestPerson.Add(entity);

            var success =
                UOW.Commit();

            return new(
                success,
                success
                    ? "پرسنل وظیفه با موفقیت ثبت شد."
                    : "ثبت پرسنل وظیفه با خطا همراه بود.");
        }

        public BaseResult DeletePerson(
            long id,
            bool canViewAll)
        {
            var user =
                Session?.GetUser();

            if (user == null)
                return new(false, "اطلاعات کاربر یافت نشد.");

            var person =
                UOW.GuestExtraFoodRequestPerson
                    .FirstOrDefault(x =>
                        x.Id == id &&
                        x.IsDeleted != true);

            if (person == null)
                return new(false, "پرسنل مورد نظر یافت نشد.");

            var request =
                GetAuthorizedRequest(
                    person.GuestExtraFoodRequestId,
                    canViewAll);

            if (request == null)
                return new(false, "درخواست یافت نشد.");

            person.IsDeleted = true;
            person.LastEditUserId = user.Id;
            person.LastEditDate = DateTime.Now;

            UOW.GuestExtraFoodRequestPerson.Update(person);

            var success =
                UOW.Commit();

            return new(
                success,
                success
                    ? "پرسنل با موفقیت حذف شد."
                    : "حذف پرسنل با خطا همراه بود.");
        }

        private GuestExtraFoodRequest GetAuthorizedRequest(
            long requestId,
            bool canViewAll)
        {
            var user =
                Session?.GetUser();

            if (user == null)
                return null;

            var request =
                UOW.GuestExtraFoodRequest
                    .FirstOrDefault(x =>
                        x.Id == requestId &&
                        x.IsDeleted != true);

            if (request == null)
                return null;

            if (!canViewAll &&
                request.OrgId != user.OmdOrgId)
            {
                return null;
            }

            return request;
        }

        private BaseResult ValidateRequestForCreate(
            GuestExtraFoodRequest request)
        {
            if (request.Status !=
                GuestExtraFoodRequestStatus.Approved)
            {
                return new(
                    false,
                    "فقط درخواست تأییدشده قابل اختصاص پرسنل است.");
            }

            if (request.ExtraQuotaCount <= 0)
            {
                return new(
                    false,
                    "این درخواست تعداد مازاد ندارد.");
            }

            var registeredCount =
                UOW.GuestExtraFoodRequestPerson
                    .GetRegisteredCount(request.Id);

            if (registeredCount >= request.ExtraQuotaCount)
            {
                return new(
                    false,
                    "تعداد پرسنل ثبت‌شده به سقف تعداد تأییدشده رسیده است.");
            }

            var today =
                DateTime.Now.Date;

            if (request.FromDate.Date > today ||
                request.ToDate.Date < today)
            {
                return new(
                    false,
                    "بازه زمانی این درخواست فعال نیست.");
            }

            return new(true, null);
        }

        private bool IsOfficial(long personalTypeId)
        {
            var personalType =
                UOW.PersonalType.FirstOrDefault(x =>
                    x.Id == personalTypeId);

            return personalType != null &&
                   personalType.Code ==
                   PersonalTypeCodes.Official;
        }

        private bool IsDuty(long personalTypeId)
        {
            var personalType =
                UOW.PersonalType.FirstOrDefault(x =>
                    x.Id == personalTypeId);

            return personalType != null &&
                   personalType.Code ==
                   PersonalTypeCodes.Duty;
        }
    }
}