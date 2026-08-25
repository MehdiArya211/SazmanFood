using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Services.SessionServices;
using Utilities.Extentions;

namespace BLL.Interface;

public class UnitStatisticManager
    : Manager<UnitStatistic, ApplicationContext>,
      IUnitStatisticManager
{
    protected readonly IHttpContextAccessor httpContextAccessor;
    protected readonly ISession Session;

    public UnitStatisticManager(
        DbContexts context,
        IHttpContextAccessor httpContextAccessor)
        : base(context, httpContextAccessor)
    {
        this.httpContextAccessor =
            httpContextAccessor ??
            throw new ArgumentNullException(
                nameof(httpContextAccessor));

        Session =
            httpContextAccessor.HttpContext?.Session;
    }

    #region دریافت لیست

    /// <summary>
    /// دریافت لیست آمار مربوط به یگان کاربر جاری
    /// </summary>
    public DataTableResponseDTO<UnitStatisticDTO>
        GetDataTableDTO(
            DataTableSearchDTO searchData,
            UnitStatisticFilterDTO filters)
    {
        var user =
            Session?.GetUser();

        if (user == null)
        {
            return new DataTableResponseDTO<
                UnitStatisticDTO>();
        }

        var userOrgId =
            Convert.ToInt32(user.OmdOrgId);

        if (userOrgId <= 0)
        {
            return new DataTableResponseDTO<
                UnitStatisticDTO>();
        }

        filters ??=
            new UnitStatisticFilterDTO();

        /*
         * OrgId ارسال‌شده از سمت مرورگر قابل اعتماد نیست.
         * کاربر فقط آمار یگان خودش را مشاهده می‌کند.
         */
        filters.OrgId =
            userOrgId;

        return UOW.UnitStatistic
            .GetDataTableDTO(
                searchData,
                filters);
    }

    #endregion

    #region ثبت اولیه

    /// <summary>
    /// ثبت اولیه آمار یگان
    /// </summary>
    public BaseResult Create(UnitStatisticCreateDTO model, long creatorId, string creatorFullName)
    {
        var oldDraft = UOW.UnitStatistic.FirstOrDefault(x =>
            x.OrgId == model.OrgId &&
            x.Status != UnitStatisticStatus.Approved);

        if (oldDraft != null)
        {
            return new BaseResult(
                false,
                "برای این یگان یک آمار ثبت اولیه یا ارسال‌شده وجود دارد.");
        }

        var entity = new UnitStatistic
        {
            OrgId = model.OrgId,
            OrgTitle = model.OrgTitle,
            TotalOfficialCount = model.TotalOfficialCount,
            TotalDutyCount = model.TotalDutyCount,
            Status = UnitStatisticStatus.Draft,
            CreatorId = creatorId,
            CreatorFullName = creatorFullName,
            CreateDate = DateTime.Now,
            ApproverId = null,
            ApproverFullName = null,
            ApproveDate = null,
            IsActive = false
        };

        return base.Create(entity);
    }

    #endregion

    #region دریافت مدل ویرایش

    /// <summary>
    /// دریافت مدل ویرایش آمار
    /// </summary>
    public UnitStatisticEditDTO GetEditDTO(long id)
    {
        if (id <= 0)
            return null;

        return UOW.UnitStatistic.GetOneDTO<UnitStatisticEditDTO>(
            UnitStatisticEditDTO.Selector,
            x => x.Id == id);
    }

    #endregion

    #region ویرایش

    /// <summary>
    /// ویرایش آمار ثبت اولیه یا ارسال‌شده
    /// </summary>
    public BaseResult Update(UnitStatisticEditDTO model)
    {
        var entity = UOW.UnitStatistic.FirstOrDefault(x => x.Id == model.Id);

        if (entity == null)
            return new BaseResult(false, "آمار مورد نظر یافت نشد.");

        if (entity.Status == UnitStatisticStatus.Approved)
            return new BaseResult(false, "آمار تأیید نهایی شده قابل ویرایش نیست.");

        entity.TotalOfficialCount = model.TotalOfficialCount;
        entity.TotalDutyCount = model.TotalDutyCount;

        /*
         * اگر آمار قبلاً ارسال شده باشد، بعد از ویرایش
         * برای ارسال مجدد به ثبت اولیه برمی‌گردد.
         */
        entity.Status = UnitStatisticStatus.Draft;

        return base.Update(entity);
    }

    #endregion

    #region دریافت جزئیات

    /// <summary>
    /// دریافت اطلاعات سربرگ و جزئیات کادر و وظیفه
    /// </summary>
    public UnitStatisticDetailsDTO GetDetailsDTO(
        long? id)
    {
        if (!id.HasValue ||
            id.Value <= 0)
        {
            return null;
        }

        var user =
            Session?.GetUser();

        if (user == null)
        {
            return null;
        }

        var userOrgId =
            Convert.ToInt32(user.OmdOrgId);

        if (userOrgId <= 0)
        {
            return null;
        }

        var statistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.Id == id.Value &&
                x.OrgId == userOrgId);

        if (statistic == null)
        {
            return null;
        }

        var officialType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code ==
                PersonalTypeCodes.Official);

        if (officialType == null)
        {
            return null;
        }

        var dutyType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code ==
                PersonalTypeCodes.Duty);

        if (dutyType == null)
        {
            return null;
        }

        var yeganTypes =
            UOW.YeganType.GetAll()
                .OrderBy(x => x.SortName)
                .ThenBy(x => x.Title)
                .ToList();

        var savedDetails =
            UOW.UnitStatisticDetail
                .GetByUnitStatisticId(
                    statistic.Id);

        var model =
            new UnitStatisticDetailsDTO
            {
                Id =
                    statistic.Id,

                OrgId =
                    statistic.OrgId,

                OrgTitle =
                    statistic.OrgTitle,

                TotalOfficialCount =
                    statistic.TotalOfficialCount,

                TotalDutyCount =
                    statistic.TotalDutyCount,

                Status =
                    statistic.Status,

                OfficialDetails =
                    new List<
                        UnitStatisticDetailItemDTO>(),

                DutyDetails =
                    new List<
                        UnitStatisticDetailItemDTO>()
            };

        foreach (var yeganType in yeganTypes)
        {
            var officialDetail =
                savedDetails.FirstOrDefault(x =>
                    x.PersonalTypeId ==
                    officialType.Id &&
                    x.YeganTypeId ==
                    yeganType.Id);

            model.OfficialDetails.Add(
                new UnitStatisticDetailItemDTO
                {
                    Id =
                        officialDetail?.Id,

                    YeganTypeId =
                        yeganType.Id,

                    YeganTypeTitle =
                        yeganType.Title,

                    Count =
                        officialDetail?.Count ?? 0
                });

            var dutyDetail =
                savedDetails.FirstOrDefault(x =>
                    x.PersonalTypeId ==
                    dutyType.Id &&
                    x.YeganTypeId ==
                    yeganType.Id);

            model.DutyDetails.Add(
                new UnitStatisticDetailItemDTO
                {
                    Id =
                        dutyDetail?.Id,

                    YeganTypeId =
                        yeganType.Id,

                    YeganTypeTitle =
                        yeganType.Title,

                    Count =
                        dutyDetail?.Count ?? 0
                });
        }

        return model;
    }

    #endregion

    #region ذخیره جزئیات

    /// <summary>
    /// ذخیره جزئیات کادر و وظیفه
    /// </summary>
    public BaseResult SaveDetails(
        UnitStatisticDetailsDTO model)
    {
        if (model == null ||
            model.Id <= 0)
        {
            return new BaseResult(
                false,
                "اطلاعات جزئیات معتبر نیست.");
        }

        var user =
            Session?.GetUser();

        if (user == null)
        {
            return new BaseResult(
                false,
                "اطلاعات کاربر جاری یافت نشد.");
        }

        var userOrgId =
            Convert.ToInt32(user.OmdOrgId);

        if (userOrgId <= 0)
        {
            return new BaseResult(
                false,
                "برای کاربر جاری یگان مشخص نشده است.");
        }

        var statistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.Id == model.Id &&
                x.OrgId == userOrgId);

        if (statistic == null)
        {
            return new BaseResult(
                false,
                "آمار مورد نظر یافت نشد.");
        }

        if (statistic.Status ==
            UnitStatisticStatus.Approved)
        {
            return new BaseResult(
                false,
                "جزئیات آمار تأییدشده قابل ویرایش نیست.");
        }

        var officialDetails =
            model.OfficialDetails ??
            new List<
                UnitStatisticDetailItemDTO>();

        var dutyDetails =
            model.DutyDetails ??
            new List<
                UnitStatisticDetailItemDTO>();

        var officialType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code ==
                PersonalTypeCodes.Official);

        if (officialType == null)
        {
            return new BaseResult(
                false,
                "نوع پرسنل کادر تعریف نشده است.");
        }

        var dutyType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code ==
                PersonalTypeCodes.Duty);

        if (dutyType == null)
        {
            return new BaseResult(
                false,
                "نوع پرسنل وظیفه تعریف نشده است.");
        }

        var yeganTypes =
            UOW.YeganType.GetAll()
                .ToList();

        if (!yeganTypes.Any())
        {
            return new BaseResult(
                false,
                "نوع خدمت تعریف نشده است.");
        }

        #region کنترل تعداد ردیف‌های جزئیات

        if (officialDetails.Count !=
            yeganTypes.Count)
        {
            return new BaseResult(
                false,
                "تمام جزئیات کادر وارد نشده است.");
        }

        if (dutyDetails.Count !=
            yeganTypes.Count)
        {
            return new BaseResult(
                false,
                "تمام جزئیات وظیفه وارد نشده است.");
        }

        #endregion

        #region کنترل نوع خدمت تکراری

        var hasDuplicateOfficial =
            officialDetails
                .GroupBy(x =>
                    x.YeganTypeId)
                .Any(x =>
                    x.Count() > 1);

        if (hasDuplicateOfficial)
        {
            return new BaseResult(
                false,
                "نوع خدمت تکراری در جزئیات کادر وجود دارد.");
        }

        var hasDuplicateDuty =
            dutyDetails
                .GroupBy(x =>
                    x.YeganTypeId)
                .Any(x =>
                    x.Count() > 1);

        if (hasDuplicateDuty)
        {
            return new BaseResult(
                false,
                "نوع خدمت تکراری در جزئیات وظیفه وجود دارد.");
        }

        #endregion

        #region کنترل معتبر بودن نوع خدمت‌ها

        var validYeganTypeIds =
            yeganTypes
                .Select(x => x.Id)
                .ToList();

        var officialYeganTypeIds =
            officialDetails
                .Select(x =>
                    x.YeganTypeId)
                .ToList();

        var dutyYeganTypeIds =
            dutyDetails
                .Select(x =>
                    x.YeganTypeId)
                .ToList();

        if (officialYeganTypeIds.Any(x =>
                !validYeganTypeIds.Contains(x)))
        {
            return new BaseResult(
                false,
                "یکی از انواع خدمت کادر معتبر نیست.");
        }

        if (dutyYeganTypeIds.Any(x =>
                !validYeganTypeIds.Contains(x)))
        {
            return new BaseResult(
                false,
                "یکی از انواع خدمت وظیفه معتبر نیست.");
        }

        foreach (var yeganTypeId in
                 validYeganTypeIds)
        {
            if (!officialYeganTypeIds.Contains(
                    yeganTypeId))
            {
                return new BaseResult(
                    false,
                    "تمام انواع خدمت کادر وارد نشده است.");
            }

            if (!dutyYeganTypeIds.Contains(
                    yeganTypeId))
            {
                return new BaseResult(
                    false,
                    "تمام انواع خدمت وظیفه وارد نشده است.");
            }
        }

        #endregion

        #region کنترل منفی نبودن تعدادها

        if (officialDetails.Any(x =>
                x.Count < 0))
        {
            return new BaseResult(
                false,
                "تعداد کادر نمی‌تواند منفی باشد.");
        }

        if (dutyDetails.Any(x =>
                x.Count < 0))
        {
            return new BaseResult(
                false,
                "تعداد وظیفه نمی‌تواند منفی باشد.");
        }

        #endregion

        #region کنترل مجموع کادر

        var officialSum =
            officialDetails.Sum(x =>
                x.Count);

        if (officialSum !=
            statistic.TotalOfficialCount)
        {
            return new BaseResult(
                false,

                $"جمع تعداد کادر واردشده ({officialSum}) " +
                $"با تعداد کل کادر ({statistic.TotalOfficialCount}) " +
                "همخوانی ندارد.");
        }

        #endregion

        #region کنترل مجموع وظیفه

        var dutySum =
            dutyDetails.Sum(x =>
                x.Count);

        if (dutySum !=
            statistic.TotalDutyCount)
        {
            return new BaseResult(
                false,

                $"جمع تعداد وظیفه واردشده ({dutySum}) " +
                $"با تعداد کل وظیفه ({statistic.TotalDutyCount}) " +
                "همخوانی ندارد.");
        }

        #endregion

        /*
         * جزئیات قبلی حذف می‌شوند ولی هنوز Commit نمی‌شود.
         */
        UOW.UnitStatisticDetail
            .RemoveByUnitStatisticId(
                statistic.Id);

        var newDetails =
            new List<UnitStatisticDetail>();

        newDetails.AddRange(
            officialDetails.Select(x =>
                new UnitStatisticDetail
                {
                    UnitStatisticId =
                        statistic.Id,

                    PersonalTypeId =
                        officialType.Id,

                    YeganTypeId =
                        x.YeganTypeId,

                    Count =
                        x.Count
                }));

        newDetails.AddRange(
            dutyDetails.Select(x =>
                new UnitStatisticDetail
                {
                    UnitStatisticId =
                        statistic.Id,

                    PersonalTypeId =
                        dutyType.Id,

                    YeganTypeId =
                        x.YeganTypeId,

                    Count =
                        x.Count
                }));

        UOW.UnitStatisticDetail
            .AddRange(newDetails);

        /*
         * اگر آمار قبلاً ارسال شده بود،
         * با تغییر جزئیات دوباره ثبت اولیه می‌شود.
         */
        statistic.Status =
            UnitStatisticStatus.Draft;

        statistic.IsActive =
            false;

        statistic.ApproverId =
            null;

        statistic.ApproverFullName =
            null;

        statistic.ApproveDate =
            null;

        UOW.UnitStatistic.Update(
            statistic);

        /*
         * حذف جزئیات قبلی، افزودن جزئیات جدید
         * و تغییر وضعیت با یک Commit انجام می‌شود.
         */
        var isSuccess =
            UOW.Commit();

        return new BaseResult
        {
            Status =
                isSuccess,

            Message =
                isSuccess
                    ? "جزئیات آمار با موفقیت ذخیره شد."
                    : "ذخیره جزئیات با خطا همراه بوده است."
        };
    }

    #endregion

    #region ارسال

    /// <summary>
    /// ارسال آمار برای تأیید
    /// </summary>
    public BaseResult Send(long id)
    {
        var user =
            Session?.GetUser();

        if (user == null)
        {
            return new BaseResult(
                false,
                "اطلاعات کاربر جاری یافت نشد.");
        }

        var userOrgId =
            Convert.ToInt32(user.OmdOrgId);

        if (userOrgId <= 0)
        {
            return new BaseResult(
                false,
                "برای کاربر جاری یگان مشخص نشده است.");
        }

        var statistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.Id == id &&
                x.OrgId == userOrgId);

        if (statistic == null)
        {
            return new BaseResult(
                false,
                "آمار مورد نظر یافت نشد.");
        }

        if (statistic.Status !=
            UnitStatisticStatus.Draft)
        {
            return new BaseResult(
                false,
                "فقط آمار ثبت اولیه قابل ارسال است.");
        }

        /*
         * جزئیات باید کامل باشد و مجموع‌ها برابر باشند.
         */
        var validationResult =
            ValidateDetailTotals(
                statistic);

        if (!validationResult.Status)
        {
            return validationResult;
        }

        statistic.Status =
            UnitStatisticStatus.Sent;

        statistic.IsActive =
            false;

        return base.Update(statistic);
    }

    #endregion

    #region تأیید و محاسبه سهمیه

    /// <summary>
    /// تأیید نهایی آمار و محاسبه سهمیه یگان
    /// </summary>
    public BaseResult Approve0(long id)
    {
        var user =
            Session?.GetUser();

        if (user == null)
        {
            return new BaseResult(
                false,
                "اطلاعات کاربر جاری یافت نشد.");
        }

        var userOrgId =
            Convert.ToInt32(user.OmdOrgId);

        if (userOrgId <= 0)
        {
            return new BaseResult(
                false,
                "برای کاربر جاری یگان مشخص نشده است.");
        }

        var statistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.Id == id &&
                x.OrgId == userOrgId);

        if (statistic == null)
        {
            return new BaseResult(
                false,
                "آمار مورد نظر یافت نشد.");
        }

        if (statistic.Status !=
            UnitStatisticStatus.Sent)
        {
            return new BaseResult(
                false,
                "فقط آمار ارسال‌شده قابل تأیید است.");
        }

        /*
         * کنترل کامل‌بودن جزئیات و مجموع‌ها.
         */
        var detailValidation =
            ValidateDetailTotals(
                statistic);

        if (!detailValidation.Status)
        {
            return detailValidation;
        }

        var details =
            UOW.UnitStatisticDetail
                .GetByUnitStatisticId(
                    statistic.Id);

        if (!details.Any())
        {
            return new BaseResult(
                false,
                "جزئیات آمار وارد نشده است.");
        }

        var foodSources =
            UOW.FoodSource.GetAll()
                .ToList();

        var personalTypes =
            UOW.PersonalType.GetAll()
                .ToList();

        var yeganTypes =
            UOW.YeganType.GetAll()
                .ToList();

        var dayTypes =
            Enum.GetValues(
                    typeof(DayType))
                .Cast<DayType>()
                .ToList();

        var newQuotas =
            new List<UnitQuota>();

        /*
         * ابتدا تمام سهمیه‌ها فقط در حافظه ساخته می‌شوند.
         * اگر مأخذی ناقص باشد هیچ تغییری در دیتابیس
         * ایجاد نخواهد شد.
         */
        foreach (var detail in details)
        {
            var personalType =
                personalTypes.FirstOrDefault(x =>
                    x.Id ==
                    detail.PersonalTypeId);

            if (personalType == null)
            {
                return new BaseResult(
                    false,
                    "نوع پرسنل یکی از جزئیات معتبر نیست.");
            }

            var yeganType =
                yeganTypes.FirstOrDefault(x =>
                    x.Id ==
                    detail.YeganTypeId);

            if (yeganType == null)
            {
                return new BaseResult(
                    false,
                    "نوع خدمت یکی از جزئیات معتبر نیست.");
            }

            foreach (var dayType in dayTypes)
            {
                var foodSource =
                    foodSources.FirstOrDefault(x =>
                        x.PersonalTypeId ==
                        detail.PersonalTypeId &&

                        x.YeganTypeId ==
                        detail.YeganTypeId &&

                        x.DayType ==
                        dayType);

                if (foodSource == null)
                {
                    return new BaseResult(
                        false,

                        "مأخذ غذایی برای " +
                        $"نوع پرسنل «{personalType.Title}»، " +
                        $"نوع خدمت «{yeganType.Title}» و " +
                        $"روز «{GetDayTypeTitle(dayType)}» " +
                        "تعریف نشده است.");
                }

                var breakfastQuota =
                    CalculateQuota(
                        detail.Count,
                        foodSource
                            .PercentBreakfast);

                var lunchQuota =
                    CalculateQuota(
                        detail.Count,
                        foodSource
                            .PercentLunch);

                var dinnerQuota =
                    CalculateQuota(
                        detail.Count,
                        foodSource
                            .PercentDinner);

                newQuotas.Add(
                    new UnitQuota
                    {
                        UnitStatisticId =
                            statistic.Id,

                        OrgId =
                            statistic.OrgId,

                        OrgTitle =
                            statistic.OrgTitle,

                        PersonalTypeId =
                            detail.PersonalTypeId,

                        YeganTypeId =
                            detail.YeganTypeId,

                        DayType =
                            dayType,

                        FoodSourceId =
                            foodSource.Id,

                        PersonnelCount =
                            detail.Count,

                        PercentBreakfast =
                            foodSource
                                .PercentBreakfast,

                        BreakfastQuota =
                            breakfastQuota,

                        PercentLunch =
                            foodSource
                                .PercentLunch,

                        LunchQuota =
                            lunchQuota,

                        PercentDinner =
                            foodSource
                                .PercentDinner,

                        DinnerQuota =
                            dinnerQuota,

                        CalculateDate =
                            DateTime.Now
                    });
            }
        }

        if (!newQuotas.Any())
        {
            return new BaseResult(
                false,
                "امکان محاسبه سهمیه وجود ندارد.");
        }

        /*
         * آمار تأییدشده فعال قبلی همین یگان
         * غیرفعال می‌شود.
         */
        var previousActiveStatistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.OrgId ==
                statistic.OrgId &&

                x.Id !=
                statistic.Id &&

                x.IsActive &&

                x.Status ==
                UnitStatisticStatus.Approved);

        if (previousActiveStatistic != null)
        {
            previousActiveStatistic.IsActive =
                false;

            UOW.UnitStatistic.Update(
                previousActiveStatistic);
        }

        /*
         * اگر به هر دلیل سهمیه قبلی برای همین آمار
         * وجود داشت حذف می‌شود.
         */
        UOW.UnitQuota
            .RemoveByStatisticId(
                statistic.Id);

        UOW.UnitQuota.AddRange(
            newQuotas);

        statistic.Status =
            UnitStatisticStatus.Approved;

        statistic.IsActive =
            true;

        statistic.ApproverId =
            user.Id;

        statistic.ApproverFullName =
            user.FullName;

        statistic.ApproveDate =
            DateTime.Now;

        UOW.UnitStatistic.Update(
            statistic);

        /*
         * غیرفعال‌شدن رکورد قبلی، تأیید رکورد جدید
         * و ذخیره سهمیه‌ها همگی با یک Commit انجام می‌شوند.
         */
        var isSuccess =
            UOW.Commit();

        return new BaseResult
        {
            Status =
                isSuccess,

            Message =
                isSuccess
                    ? "آمار با موفقیت تأیید و سهمیه یگان محاسبه شد."
                    : "تأیید آمار و محاسبه سهمیه با خطا همراه بوده است.",

            Model =
                isSuccess
                    ? statistic.Id
                    : null
        };
    }
    #region تأیید و محاسبه سهمیه

    /// <summary>
    /// تأیید نهایی آمار و محاسبه سهمیه یگان
    /// براساس وضعیت تاریخ جاری در تقویم همان یگان
    /// </summary>
    public BaseResult Approve(long id)
    {
        #region اطلاعات کاربر

        var user =
            Session?.GetUser();

        if (user == null)
        {
            return new BaseResult(
                false,
                "اطلاعات کاربر جاری یافت نشد.");
        }

        var userOrgId =
            Convert.ToInt32(
                user.OmdOrgId);

        if (userOrgId <= 0)
        {
            return new BaseResult(
                false,
                "برای کاربر جاری یگان مشخص نشده است.");
        }

        #endregion

        #region دریافت و اعتبارسنجی آمار

        var statistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.Id == id &&
                x.OrgId == userOrgId);

        if (statistic == null)
        {
            return new BaseResult(
                false,
                "آمار مورد نظر یافت نشد.");
        }

        if (statistic.Status !=
            UnitStatisticStatus.Sent)
        {
            return new BaseResult(
                false,
                "فقط آمار ارسال‌شده قابل تأیید است.");
        }

        var detailValidation =
            ValidateDetailTotals(
                statistic);

        if (!detailValidation.Status)
        {
            return detailValidation;
        }

        var details =
            UOW.UnitStatisticDetail
                .GetByUnitStatisticId(
                    statistic.Id);

        if (details == null ||
            !details.Any())
        {
            return new BaseResult(
                false,
                "جزئیات آمار وارد نشده است.");
        }

        #endregion

        #region دریافت اطلاعات موردنیاز محاسبه

        var foodSources =
            UOW.FoodSource
                .GetAll()
                .Where(x =>
                    x.IsDeleted != true)
                .ToList();

        var personalTypes =
            UOW.PersonalType
                .GetAll()
                .ToList();

        var yeganTypes =
            UOW.YeganType
                .GetAll()
                .ToList();

        if (!foodSources.Any())
        {
            return new BaseResult(
                false,
                "هیچ مأخذ غذایی فعالی تعریف نشده است.");
        }

        if (!personalTypes.Any())
        {
            return new BaseResult(
                false,
                "هیچ نوع پرسنلی تعریف نشده است.");
        }

        if (!yeganTypes.Any())
        {
            return new BaseResult(
                false,
                "هیچ نوع خدمتی تعریف نشده است.");
        }

        #endregion

        #region تشخیص نوع روز

        /*
         * تاریخ محاسبه فقط از تاریخ جاری سرور گرفته می‌شود.
         * ساعت در جستجوی تقویم تأثیری ندارد.
         */
        var calculationDate =
            DateTime.Now.Date;

        /*
         * اگر برای تاریخ جاری و یگان موردنظر
         * رکوردی در تقویم وجود نداشته باشد،
         * متد GetDayType مقدار Normal برمی‌گرداند.
         */
        var calendarDayType =
            UOW.UnitCalendar.GetDayType(
                statistic.OrgId,
                calculationDate);

        #endregion

        #region محاسبه سهمیه‌ها

        var newQuotas =
            new List<UnitQuota>();

        /*
         * برای هر ردیف جزئیات آمار فقط یک سهمیه ساخته می‌شود.
         *
         * غیرعملیاتی:
         * نوع روز از تقویم یگان خوانده می‌شود.
         *
         * عملیاتی:
         * در همه شرایط از روز عادی استفاده می‌شود.
         */
        foreach (var detail in details)
        {
            var personalType =
                personalTypes.FirstOrDefault(x =>
                    x.Id ==
                    detail.PersonalTypeId);

            if (personalType == null)
            {
                return new BaseResult(
                    false,
                    "نوع پرسنل یکی از جزئیات آمار معتبر نیست.");
            }

            var yeganType =
                yeganTypes.FirstOrDefault(x =>
                    x.Id ==
                    detail.YeganTypeId);

            if (yeganType == null)
            {
                return new BaseResult(
                    false,
                    "نوع خدمت یکی از جزئیات آمار معتبر نیست.");
            }

            /*
             * فعلاً به دلیل مشخص‌نبودن Code عملیاتی،
             * تشخیص براساس عنوان انجام می‌شود.
             */
            var isOperational =
                string.Equals(
                    yeganType.Title?.Trim(),
                    "عملیاتی",
                    StringComparison.OrdinalIgnoreCase);

            var effectiveDayType =
                isOperational
                    ? DayType.Normal
                    : calendarDayType;

            var foodSource =
                foodSources.FirstOrDefault(x =>
                    x.PersonalTypeId ==
                        detail.PersonalTypeId &&

                    x.YeganTypeId ==
                        detail.YeganTypeId &&

                    x.DayType ==
                        effectiveDayType);

            if (foodSource == null)
            {
                return new BaseResult(
                    false,
                    "مأخذ غذایی برای " +
                    $"نوع پرسنل «{personalType.Title}»، " +
                    $"نوع خدمت «{yeganType.Title}» و " +
                    $"روز «{GetDayTypeTitle(effectiveDayType)}» " +
                    "تعریف نشده است.");
            }

            var breakfastQuota =
                CalculateQuota(
                    detail.Count,
                    foodSource.PercentBreakfast);

            var lunchQuota =
                CalculateQuota(
                    detail.Count,
                    foodSource.PercentLunch);

            var dinnerQuota =
                CalculateQuota(
                    detail.Count,
                    foodSource.PercentDinner);

            var quota =
                new UnitQuota
                {
                    UnitStatisticId =
                        statistic.Id,

                    OrgId =
                        statistic.OrgId,

                    OrgTitle =
                        statistic.OrgTitle,

                    PersonalTypeId =
                        detail.PersonalTypeId,

                    YeganTypeId =
                        detail.YeganTypeId,

                    DayType =
                        effectiveDayType,

                    FoodSourceId =
                        foodSource.Id,

                    PersonnelCount =
                        detail.Count,

                    PercentBreakfast =
                        foodSource.PercentBreakfast,

                    BreakfastQuota =
                        breakfastQuota,

                    PercentLunch =
                        foodSource.PercentLunch,

                    LunchQuota =
                        lunchQuota,

                    PercentDinner =
                        foodSource.PercentDinner,

                    DinnerQuota =
                        dinnerQuota,

                    CalculateDate =
                        DateTime.Now,

                    RegUserId =
                        user.Id,

                    RegDate =
                        DateTime.Now,

                    IsDeleted =
                        false
                };

            newQuotas.Add(
                quota);
        }

        if (!newQuotas.Any())
        {
            return new BaseResult(
                false,
                "امکان محاسبه سهمیه وجود ندارد.");
        }

        #endregion

        #region کنترل سهمیه قبلی همین آمار

        /*
         * اگر برای سهمیه‌های قبلی این آمار نفر ثبت شده باشد،
         * حذف سهمیه باعث خطای Foreign Key می‌شود.
         */
        var hasRegisteredPersons =
            UOW.UnitQuota.HasRegisteredPersons(
                statistic.Id);

        if (hasRegisteredPersons)
        {
            return new BaseResult(
                false,
                "برای سهمیه قبلی این آمار نفر ثبت شده است و امکان محاسبه مجدد وجود ندارد.");
        }

        #endregion

        #region غیرفعال‌کردن آمار فعال قبلی

        var previousActiveStatistic =
            UOW.UnitStatistic.FirstOrDefault(x =>
                x.OrgId ==
                    statistic.OrgId &&

                x.Id !=
                    statistic.Id &&

                x.IsActive &&

                x.Status ==
                    UnitStatisticStatus.Approved);

        if (previousActiveStatistic != null)
        {
            previousActiveStatistic.IsActive =
                false;

            previousActiveStatistic.LastEditUserId =
                user.Id;

            previousActiveStatistic.LastEditDate =
                DateTime.Now;

            UOW.UnitStatistic.Update(
                previousActiveStatistic);
        }

        #endregion

        #region حذف نتیجه قبلی و ثبت سهمیه‌های جدید

        /*
         * اگر قبلاً برای همین آمار سهمیه محاسبه شده باشد،
         * نتایج قبلی حذف و نتایج جدید درج می‌شوند.
         */
        UOW.UnitQuota.RemoveByStatisticId(
            statistic.Id);

        UOW.UnitQuota.AddRange(
            newQuotas);

        #endregion

        #region تأیید نهایی آمار

        statistic.Status =
            UnitStatisticStatus.Approved;

        statistic.IsActive =
            true;

        statistic.ApproverId =
            user.Id;

        statistic.ApproverFullName =
            user.FullName;

        statistic.ApproveDate =
            DateTime.Now;

        statistic.LastEditUserId =
            user.Id;

        statistic.LastEditDate =
            DateTime.Now;

        UOW.UnitStatistic.Update(
            statistic);

        #endregion

        #region ذخیره نهایی

        /*
         * غیرفعال‌کردن آمار قبلی، حذف سهمیه قبلی،
         * ایجاد سهمیه‌های جدید و تأیید آمار
         * همگی با یک Commit ذخیره می‌شوند.
         */
        var isSuccess =
            UOW.Commit();

        var calendarDayTypeTitle =
            GetDayTypeTitle(
                calendarDayType);

        return new BaseResult
        {
            Status =
                isSuccess,

            Message =
                isSuccess
                    ? "آمار با موفقیت تأیید شد و " +
                      $"سهمیه تاریخ «{calculationDate.ToPersianDateTime()}» " +
                      $"براساس روز «{calendarDayTypeTitle}» محاسبه شد. " +
                      "سهمیه نوع خدمت عملیاتی براساس روز عادی محاسبه شده است."
                    : "تأیید آمار و محاسبه سهمیه با خطا همراه بوده است.",

            Model =
                isSuccess
                    ? statistic.Id
                    : null
        };

        #endregion
    }

    #endregion
    #endregion

    #region کنترل جزئیات

    /// <summary>
    /// کنترل کامل‌بودن و مجموع جزئیات کادر و وظیفه
    /// </summary>
    private BaseResult ValidateDetailTotals(
        UnitStatistic statistic)
    {
        if (statistic == null)
        {
            return new BaseResult(
                false,
                "اطلاعات آمار معتبر نیست.");
        }

        var officialType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code ==
                PersonalTypeCodes.Official);

        if (officialType == null)
        {
            return new BaseResult(
                false,
                "نوع پرسنل کادر تعریف نشده است.");
        }

        var dutyType =
            UOW.PersonalType.FirstOrDefault(x =>
                x.Code ==
                PersonalTypeCodes.Duty);

        if (dutyType == null)
        {
            return new BaseResult(
                false,
                "نوع پرسنل وظیفه تعریف نشده است.");
        }

        var yeganTypes =
            UOW.YeganType.GetAll()
                .ToList();

        if (!yeganTypes.Any())
        {
            return new BaseResult(
                false,
                "نوع خدمت تعریف نشده است.");
        }

        var details =
            UOW.UnitStatisticDetail
                .GetByUnitStatisticId(
                    statistic.Id);

        if (details == null ||
            !details.Any())
        {
            return new BaseResult(
                false,
                "جزئیات آمار یگان وارد نشده است.");
        }

        foreach (var yeganType in yeganTypes)
        {
            var officialDetail =
                details.FirstOrDefault(x =>
                    x.PersonalTypeId ==
                    officialType.Id &&

                    x.YeganTypeId ==
                    yeganType.Id);

            if (officialDetail == null)
            {
                return new BaseResult(
                    false,

                    "جزئیات کادر برای نوع خدمت " +
                    $"«{yeganType.Title}» وارد نشده است.");
            }

            var dutyDetail =
                details.FirstOrDefault(x =>
                    x.PersonalTypeId ==
                    dutyType.Id &&

                    x.YeganTypeId ==
                    yeganType.Id);

            if (dutyDetail == null)
            {
                return new BaseResult(
                    false,

                    "جزئیات وظیفه برای نوع خدمت " +
                    $"«{yeganType.Title}» وارد نشده است.");
            }
        }

        if (details.Any(x =>
                x.Count < 0))
        {
            return new BaseResult(
                false,
                "تعداد جزئیات نمی‌تواند منفی باشد.");
        }

        var officialSum =
            details
                .Where(x =>
                    x.PersonalTypeId ==
                    officialType.Id)
                .Sum(x =>
                    x.Count);

        if (officialSum !=
            statistic.TotalOfficialCount)
        {
            return new BaseResult(
                false,

                $"جمع جزئیات کادر ({officialSum}) " +
                $"با تعداد کل کادر ({statistic.TotalOfficialCount}) " +
                "برابر نیست.");
        }

        var dutySum =
            details
                .Where(x =>
                    x.PersonalTypeId ==
                    dutyType.Id)
                .Sum(x =>
                    x.Count);

        if (dutySum !=
            statistic.TotalDutyCount)
        {
            return new BaseResult(
                false,

                $"جمع جزئیات وظیفه ({dutySum}) " +
                $"با تعداد کل وظیفه ({statistic.TotalDutyCount}) " +
                "برابر نیست.");
        }

        return new BaseResult(
            true,
            null);
    }

    #endregion

    #region محاسبه سهمیه

    /// <summary>
    /// محاسبه سهمیه بر اساس تعداد و درصد مأخذ غذایی
    /// </summary>
    private int CalculateQuota(
        int personnelCount,
        double percent)
    {
        if (personnelCount <= 0 ||
            percent <= 0)
        {
            return 0;
        }

        var calculatedQuota =
            personnelCount *
            percent /
            100D;

        /*
         * سهمیه اعشاری رو به بالا گرد می‌شود.
         */
        return Convert.ToInt32(
            Math.Ceiling(
                calculatedQuota));
    }

    /// <summary>
    /// دریافت عنوان فارسی نوع روز
    /// </summary>
    private string GetDayTypeTitle(
        DayType dayType)
    {
        switch (dayType)
        {
            case DayType.Normal:
                return "عادی";

            case DayType.Holiday:
                return "تعطیل";

            case DayType.HalfHoliday:
                return "نیمه تعطیل";

            default:
                return "نامشخص";
        }
    }

    #endregion
}