using Domain.Entities;
using Domain.Enums;

namespace DAL.Interface
{
    public interface IUnitCalendarRepository
        : IRepository<UnitCalendar>
    {
        UnitCalendar GetByOrgAndDate(
            int orgId,
            DateTime calendarDate);

        bool Exists(
            int orgId,
            DateTime calendarDate,
            long? exceptId = null);

        DayType GetDayType(
            int orgId,
            DateTime calendarDate);
    }
}