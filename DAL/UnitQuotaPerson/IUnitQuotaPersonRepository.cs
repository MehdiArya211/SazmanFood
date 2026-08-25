using Domain.Entities;

namespace DAL.Interface
{
    public interface IUnitQuotaPersonRepository
        : IRepository<UnitQuotaPerson>
    {
        List<UnitQuotaPerson> GetPersons(
            long unitQuotaId,
            long mealId);

        int GetRegisteredCount(
            long unitQuotaId,
            long mealId);

        bool OfficialHasOtherYeganType(
            long unitStatisticId,
            long yeganTypeId,
            long? personId,
            string personCode);

        bool DutyHasOtherYeganType(
            long unitStatisticId,
            long yeganTypeId,
            string nationalCode,
            string personCode);

        bool OfficialHasMeal(
            long unitQuotaId,
            long mealId,
            long? personId,
            string personCode,
            long? exceptId = null);

        bool DutyHasMeal(
            long unitQuotaId,
            long mealId,
            string nationalCode,
            string personCode,
            long? exceptId = null);
    }
}