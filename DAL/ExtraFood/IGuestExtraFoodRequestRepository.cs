using Domain.Entities;

namespace DAL.Interface
{
    public interface IGuestExtraFoodRequestRepository
        : IRepository<GuestExtraFoodRequest>
    {
        GuestExtraFoodRequest GetRequest(long id);

        int GetApprovedExtraQuota(
            int orgId,
            long mealId,
            long personalTypeId,
            long yeganTypeId,
            DateTime date);
    }

    public interface IGuestExtraFoodRequestAttachmentRepository
        : IRepository<GuestExtraFoodRequestAttachment>
    {
        List<GuestExtraFoodRequestAttachment> GetByRequestId(
            long requestId);
    }
}