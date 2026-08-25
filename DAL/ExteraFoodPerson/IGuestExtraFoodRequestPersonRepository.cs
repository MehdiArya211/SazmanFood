using Domain.Entities;

namespace DAL.Interface
{
    public interface IGuestExtraFoodRequestPersonRepository
        : IRepository<GuestExtraFoodRequestPerson>
    {
        List<GuestExtraFoodRequestPerson> GetByRequestId(long requestId);

        int GetRegisteredCount(long requestId);

        bool HasPersonCode(long requestId, string personCode, long? exceptId = null);

        bool HasNationalCode(long requestId, string nationalCode, long? exceptId = null);
    }
}