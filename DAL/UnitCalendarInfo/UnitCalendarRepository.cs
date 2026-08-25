using DAL.Interface;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class UnitCalendarRepository
        : Repository<UnitCalendar>,
          IUnitCalendarRepository
    {
        public UnitCalendarRepository(
            DbContext context)
            : base(context)
        {
        }

        public UnitCalendar GetByOrgAndDate(
            int orgId,
            DateTime calendarDate)
        {
            var date =
                calendarDate.Date;

            return Entities
                .AsNoTracking()
                .FirstOrDefault(x =>
                    x.OrgId == orgId &&
                    x.CalendarDate == date &&
                    x.IsDeleted != true);
        }

        public bool Exists(
            int orgId,
            DateTime calendarDate,
            long? exceptId = null)
        {
            var date =
                calendarDate.Date;

            return Entities.Any(x =>
                x.OrgId == orgId &&
                x.CalendarDate == date &&
                x.IsDeleted != true &&
                (!exceptId.HasValue ||
                 x.Id != exceptId.Value));
        }

        public DayType GetDayType(
            int orgId,
            DateTime calendarDate)
        {
            var date =
                calendarDate.Date;

            var result = Entities
                .AsNoTracking()
                .Where(x =>
                    x.OrgId == orgId &&
                    x.CalendarDate == date &&
                    x.IsDeleted != true)
                .Select(x =>
                    (DayType?)x.DayType)
                .FirstOrDefault();

            /*
             * اگر برای تاریخ موردنظر رکوردی وجود نداشت،
             * روز عادی در نظر گرفته می‌شود.
             */
            return result ??
                   DayType.Normal;
        }
    }
}