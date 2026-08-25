using DAL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class GuestExtraFoodRequestPersonRepository
        : Repository<GuestExtraFoodRequestPerson>,
          IGuestExtraFoodRequestPersonRepository
    {
        public GuestExtraFoodRequestPersonRepository(DbContext context)
            : base(context)
        {
        }

        public List<GuestExtraFoodRequestPerson> GetByRequestId(long requestId)
        {
            return Entities
                .Include(x => x.DiningHall)
                .Where(x =>
                    x.GuestExtraFoodRequestId == requestId &&
                    x.IsDeleted != true)
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public int GetRegisteredCount(long requestId)
        {
            return Entities.Count(x =>
                x.GuestExtraFoodRequestId == requestId &&
                x.IsDeleted != true);
        }

        public bool HasPersonCode(
            long requestId,
            string personCode,
            long? exceptId = null)
        {
            personCode = personCode?.Trim();

            return Entities.Any(x =>
                x.GuestExtraFoodRequestId == requestId &&
                x.PersonCode == personCode &&
                x.IsDeleted != true &&
                (!exceptId.HasValue || x.Id != exceptId.Value));
        }

        public bool HasNationalCode(
            long requestId,
            string nationalCode,
            long? exceptId = null)
        {
            nationalCode = nationalCode?.Trim();

            return Entities.Any(x =>
                x.GuestExtraFoodRequestId == requestId &&
                x.NationalCode == nationalCode &&
                x.IsDeleted != true &&
                (!exceptId.HasValue || x.Id != exceptId.Value));
        }
    }
}