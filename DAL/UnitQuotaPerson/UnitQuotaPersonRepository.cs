using DAL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class UnitQuotaPersonRepository
        : Repository<UnitQuotaPerson>,
          IUnitQuotaPersonRepository
    {
        public UnitQuotaPersonRepository(
            DbContext context)
            : base(context)
        {
        }

        public List<UnitQuotaPerson> GetPersons(
            long unitQuotaId,
            long mealId)
        {
            return Entities
                .AsNoTracking()
                .Where(x =>
                    x.UnitQuotaId == unitQuotaId &&
                    x.MealId == mealId)
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public int GetRegisteredCount(
            long unitQuotaId,
            long mealId)
        {
            return Entities.Count(x =>
                x.UnitQuotaId == unitQuotaId &&
                x.MealId == mealId);
        }

        public bool OfficialHasOtherYeganType(
            long unitStatisticId,
            long yeganTypeId,
            long? personId,
            string personCode)
        {
            personCode = Normalize(personCode);

            return Entities.Any(x =>
                x.UnitStatisticId == unitStatisticId &&
                x.UnitQuota.YeganTypeId != yeganTypeId &&
                (
                    (
                        personId.HasValue &&
                        x.PersonId.HasValue &&
                        x.PersonId.Value == personId.Value
                    ) ||
                    (
                        personCode != null &&
                        x.PersonCode == personCode
                    )
                ));
        }

        public bool DutyHasOtherYeganType(
            long unitStatisticId,
            long yeganTypeId,
            string nationalCode,
            string personCode)
        {
            nationalCode = Normalize(nationalCode);
            personCode = Normalize(personCode);

            return Entities.Any(x =>
                x.UnitStatisticId == unitStatisticId &&
                x.UnitQuota.YeganTypeId != yeganTypeId &&
                (
                    (
                        nationalCode != null &&
                        x.NationalCode == nationalCode
                    ) ||
                    (
                        personCode != null &&
                        x.PersonCode == personCode
                    )
                ));
        }

        public bool OfficialHasMeal(
            long unitQuotaId,
            long mealId,
            long? personId,
            string personCode,
            long? exceptId = null)
        {
            personCode = Normalize(personCode);

            return Entities.Any(x =>
                x.UnitQuotaId == unitQuotaId &&
                x.MealId == mealId &&
                (!exceptId.HasValue ||
                 x.Id != exceptId.Value) &&
                (
                    (
                        personId.HasValue &&
                        x.PersonId.HasValue &&
                        x.PersonId.Value == personId.Value
                    ) ||
                    (
                        personCode != null &&
                        x.PersonCode == personCode
                    )
                ));
        }

        public bool DutyHasMeal(
            long unitQuotaId,
            long mealId,
            string nationalCode,
            string personCode,
            long? exceptId = null)
        {
            nationalCode = Normalize(nationalCode);
            personCode = Normalize(personCode);

            return Entities.Any(x =>
                x.UnitQuotaId == unitQuotaId &&
                x.MealId == mealId &&
                (!exceptId.HasValue ||
                 x.Id != exceptId.Value) &&
                (
                    (
                        nationalCode != null &&
                        x.NationalCode == nationalCode
                    ) ||
                    (
                        personCode != null &&
                        x.PersonCode == personCode
                    )
                ));
        }

        private string Normalize(string value)
        {
            value = value?.Trim();

            return string.IsNullOrWhiteSpace(value)
                ? null
                : value;
        }
    }
}