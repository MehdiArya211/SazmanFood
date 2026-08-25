using Domain.Entities;

namespace DAL.Interface
{
    public interface IUnitQuotaRepository
        : IRepository<UnitQuota>
    {
        List<UnitQuota> GetByStatisticId(
            long unitStatisticId);

        bool HasRegisteredPersons(
            long unitStatisticId);

        void RemoveByStatisticId(
            long unitStatisticId);
    }
}