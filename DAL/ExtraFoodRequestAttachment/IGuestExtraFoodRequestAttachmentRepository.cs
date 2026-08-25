using Domain.Entities;

namespace DAL.Interface
{
    public interface IGuestExtraFoodRequestAttachmentRepository0
        : IRepository<GuestExtraFoodRequestAttachment>
    {
        List<GuestExtraFoodRequestAttachment> GetByRequestId(
            long requestId);
    }
}