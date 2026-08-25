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
    public class UnitCalendarManager
        : Manager<UnitCalendar, ApplicationContext>,
          IUnitCalendarManager
    {
        private readonly ISession Session;

        public UnitCalendarManager(
            DbContexts context,
            IHttpContextAccessor httpContextAccessor)
            : base(
                context,
                httpContextAccessor)
        {
            Session =
                httpContextAccessor?
                    .HttpContext?
                    .Session;
        }

        public List<UnitCalendarDTO> GetList(
            UnitCalendarFilterDTO filters)
        {
            var user =
                Session?.GetUser();

            if (user == null ||
                user.OmdOrgId <= 0)
            {
                return new List<UnitCalendarDTO>();
            }

            filters ??=
                new UnitCalendarFilterDTO();

            var orgId =
                user.OmdOrgId;

            var query =
                UOW.UnitCalendar
                    .GetAll()
                    .Where(x =>
                        x.OrgId == orgId &&
                        x.IsDeleted != true);

            if (filters.FromDate.HasValue)
            {
                var fromDate =
                    filters.FromDate.Value.Date;

                query =
                    query.Where(x =>
                        x.CalendarDate >= fromDate);
            }

            if (filters.ToDate.HasValue)
            {
                var toDate =
                    filters.ToDate.Value.Date;

                query =
                    query.Where(x =>
                        x.CalendarDate <= toDate);
            }

            if (filters.DayType.HasValue)
            {
                query =
                    query.Where(x =>
                        x.DayType ==
                        filters.DayType.Value);
            }

            return query
                .OrderByDescending(x =>
                    x.CalendarDate)
                .ThenByDescending(x =>
                    x.Id)
                .Select(x =>
                    new UnitCalendarDTO
                    {
                        Id = x.Id,
                        OrgId = x.OrgId,
                        OrgTitle = x.OrgTitle,
                        CalendarDate =
                            x.CalendarDate,
                        DayType =
                            x.DayType,
                        Description =
                            x.Description,
                        RegDate =
                            x.RegDate
                    })
                .ToList();
        }

        public UnitCalendarEditDTO GetEditDTO(
            long id)
        {
            var user =
                Session?.GetUser();

            if (user == null ||
                user.OmdOrgId <= 0)
            {
                return null;
            }

            var entity =
                UOW.UnitCalendar.FirstOrDefault(x =>
                    x.Id == id &&
                    x.OrgId == user.OmdOrgId &&
                    x.IsDeleted != true);

            if (entity == null)
                return null;

            return new UnitCalendarEditDTO
            {
                Id = entity.Id,
                CalendarDate =
                    entity.CalendarDate,
                DayType =
                    entity.DayType,
                Description =
                    entity.Description
            };
        }

        public BaseResult Create(
            UnitCalendarCreateDTO model,
            int orgId,
            string orgTitle,
            long userId)
        {
            if (model == null)
            {
                return new BaseResult(
                    false,
                    "اطلاعات ارسالی معتبر نیست.");
            }

            if (orgId <= 0)
            {
                return new BaseResult(
                    false,
                    "یگان کاربر مشخص نشده است.");
            }

            if (!model.CalendarDate.HasValue)
            {
                return new BaseResult(
                    false,
                    "تاریخ الزامی است.");
            }

            if (!model.DayType.HasValue ||
                !Enum.IsDefined(
                    typeof(DayType),
                    model.DayType.Value))
            {
                return new BaseResult(
                    false,
                    "نوع روز معتبر نیست.");
            }

            var date =
                model.CalendarDate.Value.Date;

            if (UOW.UnitCalendar.Exists(
                    orgId,
                    date))
            {
                return new BaseResult(
                    false,
                    "برای این تاریخ قبلاً وضعیت روز ثبت شده است.");
            }

            var entity =
                new UnitCalendar
                {
                    OrgId = orgId,
                    OrgTitle =
                        orgTitle?.Trim(),
                    CalendarDate = date,
                    DayType =
                        model.DayType.Value,
                    Description =
                        model.Description?.Trim(),
                    RegUserId = userId,
                    RegDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    IsDeleted = false
                };

            UOW.UnitCalendar.Add(entity);

            var isSuccess =
                UOW.Commit();

            return new BaseResult
            {
                Status = isSuccess,

                Message = isSuccess
                    ? "وضعیت روز با موفقیت ثبت شد."
                    : "ثبت وضعیت روز با خطا همراه بود.",

                Model = isSuccess
                    ? entity.Id
                    : null
            };
        }

        public BaseResult Update(
            UnitCalendarEditDTO model,
            int orgId,
            long userId)
        {
            if (model == null ||
                model.Id <= 0)
            {
                return new BaseResult(
                    false,
                    "رکورد تقویم معتبر نیست.");
            }

            if (!model.CalendarDate.HasValue)
            {
                return new BaseResult(
                    false,
                    "تاریخ الزامی است.");
            }

            if (!model.DayType.HasValue ||
                !Enum.IsDefined(
                    typeof(DayType),
                    model.DayType.Value))
            {
                return new BaseResult(
                    false,
                    "نوع روز معتبر نیست.");
            }

            var entity =
                UOW.UnitCalendar.FirstOrDefault(x =>
                    x.Id == model.Id &&
                    x.OrgId == orgId &&
                    x.IsDeleted != true);

            if (entity == null)
            {
                return new BaseResult(
                    false,
                    "رکورد تقویم یافت نشد.");
            }

            var date =
                model.CalendarDate.Value.Date;

            if (UOW.UnitCalendar.Exists(
                    orgId,
                    date,
                    entity.Id))
            {
                return new BaseResult(
                    false,
                    "برای این تاریخ قبلاً وضعیت روز ثبت شده است.");
            }

            entity.CalendarDate = date;
            entity.DayType =
                model.DayType.Value;
            entity.Description =
                model.Description?.Trim();
            entity.LastEditUserId =
                userId;
            entity.LastEditDate =
                DateTime.Now;

            UOW.UnitCalendar.Update(entity);

            var isSuccess =
                UOW.Commit();

            return new BaseResult(
                isSuccess,
                isSuccess
                    ? "وضعیت روز با موفقیت ویرایش شد."
                    : "ویرایش وضعیت روز با خطا همراه بود.");
        }

        public BaseResult Delete(
            long id,
            int orgId,
            long userId)
        {
            var entity =
                UOW.UnitCalendar.FirstOrDefault(x =>
                    x.Id == id &&
                    x.OrgId == orgId &&
                    x.IsDeleted != true);

            if (entity == null)
            {
                return new BaseResult(
                    false,
                    "رکورد تقویم یافت نشد.");
            }

            /*
             * به علت nullable بودن IsDeleted،
             * حذف نرم صریح انجام می‌شود.
             */
            entity.IsDeleted = true;
            entity.LastEditUserId = userId;
            entity.LastEditDate = DateTime.Now;

            UOW.UnitCalendar.Update(
                entity);

            var isSuccess =
                UOW.Commit();

            return new BaseResult(
                isSuccess,
                isSuccess
                    ? "رکورد تقویم با موفقیت حذف شد."
                    : "حذف رکورد تقویم با خطا همراه بود.");
        }

        public DayType GetEffectiveDayType(
            int orgId,
            DateTime date,
            string yeganTypeTitle)
        {
            var isOperational =
                string.Equals(
                    yeganTypeTitle?.Trim(),
                    "عملیاتی",
                    StringComparison.OrdinalIgnoreCase);

            if (isOperational)
                return DayType.Normal;

            return UOW.UnitCalendar.GetDayType(
                orgId,
                date.Date);
        }
    }
}