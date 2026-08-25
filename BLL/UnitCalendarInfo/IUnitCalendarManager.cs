using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using DTO.Entities;
using Infrastructure.Data;

namespace BLL.Interface
{
    public interface IUnitCalendarManager
        : IManager<UnitCalendar, ApplicationContext>
    {
        List<UnitCalendarDTO> GetList(
            UnitCalendarFilterDTO filters);

        UnitCalendarEditDTO GetEditDTO(
            long id);

        BaseResult Create(
            UnitCalendarCreateDTO model,
            int orgId,
            string orgTitle,
            long userId);

        BaseResult Update(
            UnitCalendarEditDTO model,
            int orgId,
            long userId);

        BaseResult Delete(
            long id,
            int orgId,
            long userId);

        DayType GetEffectiveDayType(
            int orgId,
            DateTime date,
            string yeganTypeTitle);
    }
}