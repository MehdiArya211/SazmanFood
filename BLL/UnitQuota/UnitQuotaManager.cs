using BLL.Interface;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using DTO;
using DTO.Base;
using DTO.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Services.SessionServices;
using Utilities.Extentions;

namespace BLL;

public class UnitQuotaManager
    : Manager<UnitQuota, ApplicationContext>,
      IUnitQuotaManager
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ISession Session;
    private readonly IReservationUserProvisioningManager reservationUserProvisioningManager;

    public UnitQuotaManager(
        DbContexts context,
        IHttpContextAccessor httpContextAccessor,
        IReservationUserProvisioningManager reservationUserProvisioningManager)
        : base(context, httpContextAccessor)
    {
        this.httpContextAccessor =
            httpContextAccessor ??
            throw new ArgumentNullException(
                nameof(httpContextAccessor));

        Session =
            httpContextAccessor.HttpContext?.Session;

        this.reservationUserProvisioningManager =
            reservationUserProvisioningManager ??
            throw new ArgumentNullException(
                nameof(reservationUserProvisioningManager));
    }

    #region دریافت فرم ثبت کادر

    public UnitQuotaPersonFormDTO GetOfficialForm(
        long unitQuotaId,
        long mealId)
    {
        var user = Session?.GetUser();

        if (user == null)
            return null;

        var quota = UOW.UnitQuota.FirstOrDefault(x =>
            x.Id == unitQuotaId &&
            (user.RoleId == RoleConstant.Admin || x.OrgId == user.OmdOrgId));

        if (quota == null)
            return null;

        var officialType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code == PersonalTypeCodes.Official);

        if (officialType == null ||
            quota.PersonalTypeId != officialType.Id)
        {
            return null;
        }

        return GetPersonForm(
            quota,
            mealId,
            officialType.Id,
            officialType.Title,
            true);
    }

    #endregion

    #region دریافت فرم ثبت وظیفه

    public UnitQuotaPersonFormDTO GetDutyForm(
        long unitQuotaId,
        long mealId)
    {
        var user = Session?.GetUser();

        if (user == null)
            return null;

        var quota = UOW.UnitQuota.FirstOrDefault(x =>
            x.Id == unitQuotaId &&
            (user.RoleId == RoleConstant.Admin || x.OrgId == user.OmdOrgId));

        if (quota == null)
            return null;

        var dutyType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code == PersonalTypeCodes.Duty);

        if (dutyType == null ||
            quota.PersonalTypeId != dutyType.Id)
        {
            return null;
        }

        return GetPersonForm(
            quota,
            mealId,
            dutyType.Id,
            dutyType.Title,
            false);
    }

    #endregion

    #region دریافت اطلاعات مودال

    private UnitQuotaPersonFormDTO GetPersonForm(
        UnitQuota quota,
        long mealId,
        long personalTypeId,
        string personalTypeTitle,
        bool isOfficial)
    {
        var statistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.Id == quota.UnitStatisticId &&
                x.Status == UnitStatisticStatus.Approved &&
                x.IsActive);

        if (statistic == null)
            return null;

        var meal = UOW.Meal.FirstOrDefault(x =>
            x.Id == mealId);

        if (meal == null)
            return null;

        var yeganType =
            UOW.YeganType.FirstOrDefault(x =>
                x.Id == quota.YeganTypeId);

        if (yeganType == null)
            return null;

        var quotaCount = GetMealQuota(
            quota,
            meal.Code);

        var registeredCount =
            UOW.UnitQuotaPerson.GetRegisteredCount(
                quota.Id,
                meal.Id);

        return new UnitQuotaPersonFormDTO
        {
            UnitQuotaId = quota.Id,
            UnitStatisticId = quota.UnitStatisticId,
            OrgId = quota.OrgId,
            OrgTitle = quota.OrgTitle,
            YeganTypeId = quota.YeganTypeId,
            YeganTypeTitle = yeganType.Title,
            PersonalTypeId = personalTypeId,
            PersonalTypeTitle = personalTypeTitle,
            MealId = meal.Id,
            MealTitle = meal.Title,
            DayType = quota.DayType,
            DayTypeTitle = GetDayTypeTitle(quota.DayType),
            QuotaCount = quotaCount,
            RegisteredCount = registeredCount,
            IsOfficial = isOfficial,
            IsDuty = !isOfficial,
            Persons = GetPersons(quota.Id, meal.Id)
        };
    }

    #endregion

    #region ثبت کادر

    public BaseResult CreateOfficial(
        UnitQuotaOfficialCreateDTO model,
        PersonalInfDTO person)
    {
        if (model == null)
        {
            return new BaseResult(
                false,
                "اطلاعات ارسالی معتبر نیست.");
        }

        var user = Session?.GetUser();

        if (user == null)
        {
            return new BaseResult(
                false,
                "اطلاعات کاربر جاری یافت نشد.");
        }

        if (person == null)
        {
            return new BaseResult(
                false,
                "پرسنلی با این کد پرسنلی یافت نشد.");
        }

        var quota = UOW.UnitQuota.FirstOrDefault(x =>
            x.Id == model.UnitQuotaId &&
            (user.RoleId == RoleConstant.Admin || x.OrgId == user.OmdOrgId));

        if (quota == null)
        {
            return new BaseResult(
                false,
                "سهمیه مورد نظر یافت نشد.");
        }

        var statistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.Id == quota.UnitStatisticId &&
                x.IsActive &&
                x.Status == UnitStatisticStatus.Approved);

        if (statistic == null)
        {
            return new BaseResult(
                false,
                "سهمیه فعال و تأییدشده‌ای برای این آمار وجود ندارد.");
        }

        var officialType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code == PersonalTypeCodes.Official);

        if (officialType == null)
        {
            return new BaseResult(
                false,
                "نوع پرسنل کادر تعریف نشده است.");
        }

        if (quota.PersonalTypeId != officialType.Id)
        {
            return new BaseResult(
                false,
                "این سهمیه مربوط به کارکنان کادر نیست.");
        }

        if (model.MealId <= 0)
        {
            return new BaseResult(
                false,
                "وعده غذایی مشخص نشده است.");
        }

        var meal = UOW.Meal.FirstOrDefault(x =>
            x.Id == model.MealId);

        if (meal == null)
        {
            return new BaseResult(
                false,
                "وعده غذایی معتبر نیست.");
        }

        var validMeal =
            meal.Code == MealCodes.Breakfast ||
            meal.Code == MealCodes.Lunch ||
            meal.Code == MealCodes.Dinner;

        if (!validMeal)
        {
            return new BaseResult(
                false,
                "کد وعده غذایی معتبر نیست.");
        }

        var quotaCount = GetMealQuota(
            quota,
            meal.Code);

        if (quotaCount <= 0)
        {
            return new BaseResult(
                false,
                $"برای وعده «{meal.Title}» سهمیه‌ای وجود ندارد.");
        }

        //if (person.UnitCode != quota.OrgId)
        //{
        //    return new BaseResult(
        //        false,
        //        "این پرسنل متعلق به یگان سهمیه نیست.");
        //}

        var fullName =
            !string.IsNullOrWhiteSpace(person.FullName)
                ? person.FullName.Trim()
                : $"{person.FirstName} {person.LastName}".Trim();

        var personCode =
            person.personalCode?
                .Trim()
                .ToEnglishNumber();

        var nationalCode =
            person.MelliCode?
                .Trim()
                .ToEnglishNumber();

        if (string.IsNullOrWhiteSpace(personCode))
            personCode = null;

        if (string.IsNullOrWhiteSpace(nationalCode))
            nationalCode = null;

        model.PersonId =
            person.Id > 0
                ? person.Id
                : null;

        model.PersonCode = personCode;
        model.NationalCode = nationalCode;
        model.FullName = fullName;
        model.RankTitle = person.RankTitle?.Trim();

        if (string.IsNullOrWhiteSpace(model.PersonCode))
        {
            return new BaseResult(
                false,
                "کد پرسنلی دریافتی از وب‌سرویس معتبر نیست.");
        }

        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            return new BaseResult(
                false,
                "نام و نشان پرسنل از وب‌سرویس دریافت نشد.");
        }

        var diningHallResult =
            ValidateDiningHall(
                model.DiningHallId,
                quota.OrgId,
                officialType.Id);

        if (!diningHallResult.Status)
            return diningHallResult;

        /*
         * شخص نباید قبلاً در نوع خدمت دیگری
         * مانند عملیاتی، عادی یا آموزشی ثبت شده باشد.
         */
        var hasOtherYeganType =
            UOW.UnitQuotaPerson
                .OfficialHasOtherYeganType(
                    quota.UnitStatisticId,
                    quota.YeganTypeId,
                    model.PersonId,
                    model.PersonCode);

        if (hasOtherYeganType)
        {
            return new BaseResult(
                false,
                $"پرسنل «{model.FullName}» قبلاً در نوع سهمیه دیگری ثبت شده است.");
        }

        /*
         * جلوگیری از ثبت دوباره همان وعده
         * در همین سهمیه.
         */
        var hasMeal =
            UOW.UnitQuotaPerson.OfficialHasMeal(
                quota.Id,
                meal.Id,
                model.PersonId,
                model.PersonCode);

        if (hasMeal)
        {
            return new BaseResult(
                false,
                $"پرسنل «{model.FullName}» قبلاً برای وعده «{meal.Title}» در این سهمیه ثبت شده است.");
        }

        var registeredCount =
            UOW.UnitQuotaPerson
                .GetRegisteredCount(
                    quota.Id,
                    meal.Id);

        if (registeredCount >= quotaCount)
        {
            return new BaseResult(
                false,
                $"ظرفیت سهمیه وعده «{meal.Title}» تکمیل شده است.");
        }

        var entity = new UnitQuotaPerson
        {
            UnitStatisticId =
                quota.UnitStatisticId,

            UnitQuotaId =
                quota.Id,

            OrgId =
                quota.OrgId,

            DayType =
                quota.DayType,

            MealId =
                meal.Id,

            PersonalTypeId =
                officialType.Id,

            DiningHallId =
                model.DiningHallId.Value,

            PersonId =
                model.PersonId,

            PersonCode =
                model.PersonCode,

            NationalCode =
                model.NationalCode,

            RankTitle =
                model.RankTitle,

            FullName =
                model.FullName,

            CreatorId =
                user.Id,

            CreatorFullName =
                user.FullName,

            CreateDate =
                DateTime.Now
        };

        UOW.UnitQuotaPerson.Add(entity);

        var isSuccess = UOW.Commit();

        if (isSuccess)
        {
            var userResult =
                reservationUserProvisioningManager.EnsureReservationUser(
                    entity.PersonId,
                    entity.PersonCode,
                    entity.FullName,
                    entity.NationalCode,
                    null,
                    user.Id,
                    entity.OrgId);

            if (!userResult.Status)
            {
                return new BaseResult(
                    false,
                    $"سهمیه غذا ثبت شد، اما ساخت حساب کاربری انجام نشد: {userResult.Message}");
            }
        }

        return new BaseResult
        {
            Status = isSuccess,

            Message = isSuccess
                ? "پرسنل کادر با موفقیت ثبت شد."
                : "ثبت پرسنل کادر با خطا همراه بوده است.",

            Model = isSuccess
                ? entity.Id
                : null
        };
    }

    #endregion

    #region ثبت وظیفه

    public BaseResult CreateDuty(
        UnitQuotaDutyCreateDTO model)
    {
        if (model == null)
        {
            return new BaseResult(
                false,
                "اطلاعات ارسالی معتبر نیست.");
        }

        var user = Session?.GetUser();

        if (user == null)
        {
            return new BaseResult(
                false,
                "اطلاعات کاربر جاری یافت نشد.");
        }

        var quota = UOW.UnitQuota.FirstOrDefault(x =>
            x.Id == model.UnitQuotaId &&
            (user.RoleId == RoleConstant.Admin || x.OrgId == user.OmdOrgId));

        if (quota == null)
        {
            return new BaseResult(
                false,
                "سهمیه مورد نظر یافت نشد.");
        }

        var statistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.Id == quota.UnitStatisticId &&
                x.IsActive &&
                x.Status == UnitStatisticStatus.Approved);

        if (statistic == null)
        {
            return new BaseResult(
                false,
                "سهمیه فعال و تأییدشده‌ای برای این آمار وجود ندارد.");
        }

        var dutyType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code == PersonalTypeCodes.Duty);

        if (dutyType == null)
        {
            return new BaseResult(
                false,
                "نوع پرسنل وظیفه تعریف نشده است.");
        }

        if (quota.PersonalTypeId != dutyType.Id)
        {
            return new BaseResult(
                false,
                "این سهمیه مربوط به کارکنان وظیفه نیست.");
        }

        if (model.MealId <= 0)
        {
            return new BaseResult(
                false,
                "وعده غذایی مشخص نشده است.");
        }

        var meal = UOW.Meal.FirstOrDefault(x =>
            x.Id == model.MealId);

        if (meal == null)
        {
            return new BaseResult(
                false,
                "وعده غذایی معتبر نیست.");
        }

        var validMeal =
            meal.Code == MealCodes.Breakfast ||
            meal.Code == MealCodes.Lunch ||
            meal.Code == MealCodes.Dinner;

        if (!validMeal)
        {
            return new BaseResult(
                false,
                "کد وعده غذایی معتبر نیست.");
        }

        var quotaCount =
            GetMealQuota(
                quota,
                meal.Code);

        if (quotaCount <= 0)
        {
            return new BaseResult(
                false,
                $"برای وعده «{meal.Title}» سهمیه‌ای وجود ندارد.");
        }

        model.NationalCode =
            model.NationalCode?
                .Trim()
                .ToEnglishNumber();

        model.PersonCode =
            model.PersonCode?
                .Trim()
                .ToEnglishNumber();

        model.FullName =
            model.FullName?
                .Trim()
                .ToPersianCharacter();

        if (string.IsNullOrWhiteSpace(
                model.NationalCode) ||
            model.NationalCode.Length != 10)
        {
            return new BaseResult(
                false,
                "کد ملی باید 10 رقم باشد.");
        }

        if (string.IsNullOrWhiteSpace(
                model.PersonCode))
        {
            return new BaseResult(
                false,
                "کد پرسنلی الزامی است.");
        }

        if (string.IsNullOrWhiteSpace(
                model.FullName))
        {
            return new BaseResult(
                false,
                "نام و نشان الزامی است.");
        }

        var diningHallResult =
            ValidateDiningHall(
                model.DiningHallId,
                quota.OrgId,
                dutyType.Id);

        if (!diningHallResult.Status)
            return diningHallResult;

        var hasOtherYeganType =
            UOW.UnitQuotaPerson
                .DutyHasOtherYeganType(
                    quota.UnitStatisticId,
                    quota.YeganTypeId,
                    model.NationalCode,
                    model.PersonCode);

        if (hasOtherYeganType)
        {
            return new BaseResult(
                false,
                $"پرسنل «{model.FullName}» قبلاً در نوع سهمیه دیگری ثبت شده است.");
        }

        var hasMeal =
            UOW.UnitQuotaPerson.DutyHasMeal(
                quota.Id,
                meal.Id,
                model.NationalCode,
                model.PersonCode);

        if (hasMeal)
        {
            return new BaseResult(
                false,
                $"پرسنل «{model.FullName}» قبلاً برای وعده «{meal.Title}» در این سهمیه ثبت شده است.");
        }

        var registeredCount =
            UOW.UnitQuotaPerson
                .GetRegisteredCount(
                    quota.Id,
                    meal.Id);

        if (registeredCount >= quotaCount)
        {
            return new BaseResult(
                false,
                $"ظرفیت سهمیه وعده «{meal.Title}» تکمیل شده است.");
        }

        var entity = new UnitQuotaPerson
        {
            UnitStatisticId =
                quota.UnitStatisticId,

            UnitQuotaId =
                quota.Id,

            OrgId =
                quota.OrgId,

            DayType =
                quota.DayType,

            MealId =
                meal.Id,

            PersonalTypeId =
                dutyType.Id,

            DiningHallId =
                model.DiningHallId.Value,

            PersonId =
                null,

            PersonCode =
                model.PersonCode,

            NationalCode =
                model.NationalCode,

            RankTitle =
                "وظیفه",

            FullName =
                model.FullName,

            CreatorId =
                user.Id,

            CreatorFullName =
                user.FullName,

            CreateDate =
                DateTime.Now
        };

        UOW.UnitQuotaPerson.Add(entity);

        var isSuccess = UOW.Commit();

        if (isSuccess)
        {
            var userResult =
                reservationUserProvisioningManager.EnsureReservationUser(
                    entity.PersonId,
                    entity.PersonCode,
                    entity.FullName,
                    entity.NationalCode,
                    null,
                    user.Id,
                    entity.OrgId);

            if (!userResult.Status)
            {
                return new BaseResult(
                    false,
                    $"سهمیه غذا ثبت شد، اما ساخت حساب کاربری انجام نشد: {userResult.Message}");
            }
        }

        return new BaseResult
        {
            Status = isSuccess,

            Message = isSuccess
                ? "پرسنل وظیفه با موفقیت ثبت شد."
                : "ثبت پرسنل وظیفه با خطا همراه بوده است.",

            Model = isSuccess
                ? entity.Id
                : null
        };
    }

    #endregion

    #region دریافت نفرات

    public List<UnitQuotaPersonDTO> GetPersons(
        long unitQuotaId,
        long mealId)
    {
        var user = Session?.GetUser();

        if (user == null)
            return new List<UnitQuotaPersonDTO>();

        var quota = UOW.UnitQuota.FirstOrDefault(x =>
            x.Id == unitQuotaId &&
            (user.RoleId == RoleConstant.Admin || x.OrgId == user.OmdOrgId));

        if (quota == null)
            return new List<UnitQuotaPersonDTO>();

        var persons =
            UOW.UnitQuotaPerson.GetPersons(
                unitQuotaId,
                mealId);

        var diningHalls =
            UOW.diningHall.GetAll()
                .Where(x =>
                    x.OrgId == quota.OrgId)
                .ToList();

        return persons.Select(x =>
            new UnitQuotaPersonDTO
            {
                Id = x.Id,
                UnitQuotaId = x.UnitQuotaId,
                MealId = x.MealId,
                NationalCode = x.NationalCode,
                PersonCode = x.PersonCode,
                RankTitle = x.RankTitle,
                FullName = x.FullName,
                DiningHallId = x.DiningHallId,
                DiningHallTitle =
                    diningHalls
                        .Where(h =>
                            h.Id == x.DiningHallId)
                        .Select(h => h.Title)
                        .FirstOrDefault(),
                CreateDate = x.CreateDate
            }).ToList();
    }

    #endregion

    #region دریافت سالن‌ها

    public IList<SelectListDTO> GetDiningHalls(
        int orgId,
        long personalTypeId)
    {
        var user = Session?.GetUser();

        if (user == null ||
            (user.RoleId != RoleConstant.Admin && user.OmdOrgId != orgId))
        {
            return new List<SelectListDTO>();
        }

        return UOW.diningHall
            .GetDTO<SelectListDTO>(
                model => new SelectListDTO
                {
                    Id = model.Id,
                    Title = model.Title
                },
                x =>
                    x.OrgId == orgId &&
                    x.PersonalTypeId == personalTypeId &&
                    x.IsActive)
            .ToList();
    }

    #endregion

    #region حذف نفر

    public BaseResult DeletePerson(long id)
    {
        var user = Session?.GetUser();

        if (user == null)
        {
            return new BaseResult(
                false,
                "اطلاعات کاربر جاری یافت نشد.");
        }

        var person =
            UOW.UnitQuotaPerson.FirstOrDefault(x =>
                x.Id == id);

        if (person == null)
        {
            return new BaseResult(
                false,
                "اطلاعات فرد مورد نظر یافت نشد.");
        }

        if (user.RoleId != RoleConstant.Admin && person.OrgId != user.OmdOrgId)
        {
            return new BaseResult(
                false,
                "شما اجازه حذف این فرد را ندارید.");
        }

        UOW.UnitQuotaPerson.Remove(person);

        var isSuccess = UOW.Commit();

        return new BaseResult
        {
            Status = isSuccess,
            Message = isSuccess
                ? "فرد مورد نظر با موفقیت حذف شد."
                : "حذف فرد با خطا همراه بوده است."
        };
    }

    #endregion

    #region اعتبارسنجی سالن

    private BaseResult ValidateDiningHall(
        long? diningHallId,
        int orgId,
        long personalTypeId)
    {
        if (!diningHallId.HasValue ||
            diningHallId.Value <= 0)
        {
            return new BaseResult(
                false,
                "سالن غذاخوری را انتخاب کنید.");
        }

        var diningHall =
            UOW.diningHall.FirstOrDefault(x =>
                x.Id == diningHallId.Value);

        if (diningHall == null)
        {
            return new BaseResult(
                false,
                "سالن غذاخوری معتبر نیست.");
        }

        if (!diningHall.IsActive)
        {
            return new BaseResult(
                false,
                "سالن غذاخوری انتخاب‌شده غیرفعال است.");
        }

        if (diningHall.OrgId != orgId)
        {
            return new BaseResult(
                false,
                "سالن غذاخوری مربوط به یگان سهمیه نیست.");
        }

        if (diningHall.PersonalTypeId !=
            personalTypeId)
        {
            return new BaseResult(
                false,
                "نوع پرسنل سالن غذاخوری با سهمیه مطابقت ندارد.");
        }

        return new BaseResult(true, null);
    }

    #endregion

    #region دریافت تعداد سهمیه وعده

    private int GetMealQuota(
        UnitQuota quota,
        int mealCode)
    {
        if (quota == null)
            return 0;

        return mealCode switch
        {
            MealCodes.Breakfast =>
                quota.BreakfastQuota,

            MealCodes.Lunch =>
                quota.LunchQuota,

            MealCodes.Dinner =>
                quota.DinnerQuota,

            _ => 0
        };
    }

    #endregion

    #region عنوان نوع روز

    private string GetDayTypeTitle(
        DayType dayType)
    {
        return dayType switch
        {
            DayType.Normal => "عادی",
            DayType.Holiday => "تعطیل",
            DayType.HalfHoliday => "نیمه تعطیل",
            _ => "نامشخص"
        };
    }

    #endregion

    #region دریافت لیست سهمیه‌ها

    public List<UnitQuotaDTO> GetList(
        UnitQuotaFilterDTO filters,
        bool canViewAllOrganizations)
    {
        var result = new List<UnitQuotaDTO>();

        var user = Session?.GetUser();

        if (user == null)
            return result;

        if (user.OmdOrgId <= 0 &&
            !canViewAllOrganizations)
        {
            return result;
        }

        filters ??= new UnitQuotaFilterDTO();

        /*
         * فقط آمارهای تأییدشده و فعال.
         */
        var activeStatisticIds =
            UOW.UnitStatistic.GetAll()
                .Where(x =>
                    x.Status ==
                    UnitStatisticStatus.Approved &&
                    x.IsActive)
                .Select(x => x.Id)
                .ToList();

        if (!activeStatisticIds.Any())
            return result;

        var quotas =
            UOW.UnitQuota.GetAll()
                .Where(x =>
                    activeStatisticIds.Contains(
                        x.UnitStatisticId))
                .ToList();

        /*
         * کاربر عادی فقط یگان خودش را می‌بیند.
         */
        if (!canViewAllOrganizations)
        {
            quotas = quotas
                .Where(x =>
                    (user.RoleId == RoleConstant.Admin || x.OrgId == user.OmdOrgId))
                .ToList();
        }
        else if (filters.OrgId.HasValue &&
                 filters.OrgId.Value > 0)
        {
            quotas = quotas
                .Where(x =>
                    x.OrgId ==
                    filters.OrgId.Value)
                .ToList();
        }

        if (filters.YeganTypeId.HasValue &&
            filters.YeganTypeId.Value > 0)
        {
            quotas = quotas
                .Where(x =>
                    x.YeganTypeId ==
                    filters.YeganTypeId.Value)
                .ToList();
        }

        if (filters.DayType.HasValue)
        {
            quotas = quotas
                .Where(x =>
                    x.DayType ==
                    filters.DayType.Value)
                .ToList();
        }

        if (!quotas.Any())
            return result;

        var personalTypes =
            UOW.PersonalType.GetAll()
                .ToList();

        var officialType =
            personalTypes.FirstOrDefault(x =>
                x.Code ==
                PersonalTypeCodes.Official);

        var dutyType =
            personalTypes.FirstOrDefault(x =>
                x.Code ==
                PersonalTypeCodes.Duty);

        if (officialType == null ||
            dutyType == null)
        {
            return result;
        }

        var yeganTypes =
            UOW.YeganType.GetAll()
                .ToList();

        var meals =
            UOW.Meal.GetAll()
                .Where(x =>
                    x.Code == MealCodes.Breakfast ||
                    x.Code == MealCodes.Lunch ||
                    x.Code == MealCodes.Dinner)
                .OrderBy(x => x.SortName)
                .ToList();

        if (!meals.Any())
            return result;

        /*
         * ابتدا سهمیه هر نوع پرسنل برای هر وعده
         * به ردیف مستقل تبدیل می‌شود.
         */
        var mealItems =
            new List<UnitQuotaMealItemDTO>();

        foreach (var quota in quotas)
        {
            var personalType =
                personalTypes.FirstOrDefault(x =>
                    x.Id == quota.PersonalTypeId);

            if (personalType == null)
                continue;

            var yeganType =
                yeganTypes.FirstOrDefault(x =>
                    x.Id == quota.YeganTypeId);

            if (yeganType == null)
                continue;

            foreach (var meal in meals)
            {
                if (filters.MealId.HasValue &&
                    filters.MealId.Value > 0 &&
                    meal.Id != filters.MealId.Value)
                {
                    continue;
                }

                var quotaCount =
                    GetMealQuota(
                        quota,
                        meal.Code);

                /*
                 * سهمیه‌های صفر در گرید نمایش داده نمی‌شوند.
                 */
                if (quotaCount <= 0)
                    continue;

                mealItems.Add(
                    new UnitQuotaMealItemDTO
                    {
                        UnitQuotaId =
                            quota.Id,

                        UnitStatisticId =
                            quota.UnitStatisticId,

                        OrgId =
                            quota.OrgId,

                        OrgTitle =
                            quota.OrgTitle,

                        PersonalTypeId =
                            quota.PersonalTypeId,

                        PersonalTypeTitle =
                            personalType.Title,

                        YeganTypeId =
                            quota.YeganTypeId,

                        YeganTypeTitle =
                            yeganType.Title,

                        DayType =
                            quota.DayType,

                        MealId =
                            meal.Id,

                        MealTitle =
                            meal.Title,

                        QuotaCount =
                            quotaCount
                    });
            }
        }

        if (!mealItems.Any())
            return result;

        /*
         * کادر و وظیفه در یک ردیف گرید تجمیع می‌شوند.
         */
        var groupedItems =
            mealItems.GroupBy(x => new
            {
                x.UnitStatisticId,
                x.OrgId,
                x.OrgTitle,
                x.YeganTypeId,
                x.YeganTypeTitle,
                x.DayType,
                x.MealId,
                x.MealTitle
            });

        var row = 0;

        foreach (var group in groupedItems)
        {
            var officialItem =
                group.FirstOrDefault(x =>
                    x.PersonalTypeId ==
                    officialType.Id);

            var dutyItem =
                group.FirstOrDefault(x =>
                    x.PersonalTypeId ==
                    dutyType.Id);

            /*
             * اگر برای هیچ‌کدام سهمیه وجود نداشت،
             * ردیفی ایجاد نمی‌شود.
             */
            if (officialItem == null &&
                dutyItem == null)
            {
                continue;
            }

            var officialRegisteredCount =
                officialItem == null
                    ? 0
                    : UOW.UnitQuotaPerson
                        .GetRegisteredCount(
                            officialItem.UnitQuotaId,
                            group.Key.MealId);

            var dutyRegisteredCount =
                dutyItem == null
                    ? 0
                    : UOW.UnitQuotaPerson
                        .GetRegisteredCount(
                            dutyItem.UnitQuotaId,
                            group.Key.MealId);

            row++;

            result.Add(
                new UnitQuotaDTO
                {
                    Row =
                        row,

                    OrgId =
                        group.Key.OrgId,

                    OrgTitle =
                        group.Key.OrgTitle,

                    YeganTypeId =
                        group.Key.YeganTypeId,

                    YeganTypeTitle =
                        group.Key.YeganTypeTitle,

                    MealId =
                        group.Key.MealId,

                    MealTitle =
                        group.Key.MealTitle,

                    DayType =
                        group.Key.DayType,

                    DayTypeTitle =
                        GetDayTypeTitle(
                            group.Key.DayType),

                    OfficialUnitQuotaId =
                        officialItem?.UnitQuotaId,

                    OfficialQuotaCount =
                        officialItem?.QuotaCount ?? 0,

                    OfficialRegisteredCount =
                        officialRegisteredCount,

                    DutyUnitQuotaId =
                        dutyItem?.UnitQuotaId,

                    DutyQuotaCount =
                        dutyItem?.QuotaCount ?? 0,

                    DutyRegisteredCount =
                        dutyRegisteredCount,

                    TotalQuotaCount =
                        (officialItem?.QuotaCount ?? 0) +
                        (dutyItem?.QuotaCount ?? 0)
                });
        }

        return result
            .OrderBy(x => x.OrgTitle)
            .ThenBy(x => x.YeganTypeTitle)
            .ThenBy(x => x.DayType)
            .ThenBy(x => x.MealId)
            .Select((item, index) =>
            {
                item.Row = index + 1;
                return item;
            })
            .ToList();
    }

    #endregion

    #region سهمیه قابل رزرو کاربر

    /// <summary>
    /// تاریخ و وعده‌های قابل رزرو کاربر را از سیستم جدید
    /// UnitQuota / UnitQuotaPerson استخراج می‌کند.
    /// </summary>
    public IReadOnlyCollection<(DateTime Date, long MealId)> GetReservableSlots(
        long? personId,
        string personCode,
        string nationalCode,
        int orgId,
        DateTime fromDate,
        DateTime toDate)
    {
        personCode =
            NormalizeReservationIdentity(personCode);

        nationalCode =
            NormalizeReservationIdentity(nationalCode);

        fromDate =
            fromDate.Date;

        toDate =
            toDate.Date;

        if (fromDate > toDate)
        {
            return Array.Empty<(DateTime Date, long MealId)>();
        }

        if ((!personId.HasValue || personId.Value <= 0) &&
            string.IsNullOrWhiteSpace(personCode) &&
            string.IsNullOrWhiteSpace(nationalCode))
        {
            return Array.Empty<(DateTime Date, long MealId)>();
        }

        var activeStatisticIds =
            UOW.UnitStatistic.GetAll()
                .Where(x =>
                    x.Status == UnitStatisticStatus.Approved &&
                    x.IsActive &&
                    (orgId <= 0 || x.OrgId == orgId))
                .Select(x => x.Id)
                .ToList();

        if (!activeStatisticIds.Any())
        {
            return Array.Empty<(DateTime Date, long MealId)>();
        }

        var registrations =
            UOW.UnitQuotaPerson.GetAll()
                .Where(x =>
                    activeStatisticIds.Contains(x.UnitStatisticId) &&
                    (orgId <= 0 || x.OrgId == orgId) &&
                    (
                        (personId.HasValue &&
                         personId.Value > 0 &&
                         x.PersonId.HasValue &&
                         x.PersonId.Value == personId.Value) ||

                        (!string.IsNullOrWhiteSpace(personCode) &&
                         x.PersonCode == personCode) ||

                        (!string.IsNullOrWhiteSpace(nationalCode) &&
                         x.NationalCode == nationalCode)
                    ))
                .ToList();

        if (!registrations.Any())
        {
            return Array.Empty<(DateTime Date, long MealId)>();
        }

        var quotaIds =
            registrations
                .Select(x => x.UnitQuotaId)
                .Distinct()
                .ToList();

        var quotas =
            UOW.UnitQuota.GetAll()
                .Where(x =>
                    quotaIds.Contains(x.Id) &&
                    activeStatisticIds.Contains(x.UnitStatisticId))
                .ToDictionary(x => x.Id);

        if (!quotas.Any())
        {
            return Array.Empty<(DateTime Date, long MealId)>();
        }

        var yeganTypeIds =
            quotas.Values
                .Select(x => x.YeganTypeId)
                .Distinct()
                .ToList();

        var operationalYeganTypeIds =
            UOW.YeganType.GetAll()
                .Where(x =>
                    yeganTypeIds.Contains(x.Id) &&
                    x.Title != null &&
                    x.Title.Trim() == "عملیاتی")
                .Select(x => x.Id)
                .ToHashSet();

        var result =
            new HashSet<(DateTime Date, long MealId)>();

        for (var date = fromDate;
             date <= toDate;
             date = date.AddDays(1))
        {
            foreach (var registration in registrations)
            {
                if (!quotas.TryGetValue(
                        registration.UnitQuotaId,
                        out var quota))
                {
                    continue;
                }

                var effectiveDayType =
                    operationalYeganTypeIds.Contains(
                        quota.YeganTypeId)
                        ? DayType.Normal
                        : UOW.UnitCalendar.GetDayType(
                            registration.OrgId,
                            date);

                if (registration.DayType != effectiveDayType ||
                    quota.DayType != effectiveDayType)
                {
                    continue;
                }

                result.Add((
                    date.Date,
                    registration.MealId));
            }
        }

        return result
            .OrderBy(x => x.Date)
            .ThenBy(x => x.MealId)
            .ToList();
    }

    private static string NormalizeReservationIdentity(
        string value)
    {
        value =
            value?
                .Trim()
                .ToEnglishNumber();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    #endregion
}