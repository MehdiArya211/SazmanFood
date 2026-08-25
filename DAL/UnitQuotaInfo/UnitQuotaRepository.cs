using DAL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class UnitQuotaRepository
        : Repository<UnitQuota>,
          IUnitQuotaRepository
    {
        public UnitQuotaRepository(
            DbContext context)
            : base(context)
        {
        }

        public List<UnitQuota> GetByStatisticId(
            long unitStatisticId)
        {
            return Entities
                .AsNoTracking()
                .Where(x =>
                    x.UnitStatisticId ==
                        unitStatisticId &&
                    x.IsDeleted != true)
                .ToList();
        }

        /// <summary>
        /// بررسی می‌کند آیا برای سهمیه‌های این آمار
        /// شخصی ثبت شده است یا خیر.
        /// </summary>
        public bool HasRegisteredPersons(
            long unitStatisticId)
        {
            return Entities.Any(x =>
                x.UnitStatisticId ==
                    unitStatisticId &&
                x.IsDeleted != true &&
                x.Persons.Any());
        }

        public void RemoveByStatisticId(
            long unitStatisticId)
        {
            var quotas =
                Entities
                    .Where(x =>
                        x.UnitStatisticId ==
                        unitStatisticId)
                    .ToList();

            if (quotas.Any())
            {
                Entities.RemoveRange(
                    quotas);
            }
        }
    }
}