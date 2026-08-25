using DAL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class GuestExtraFoodRequestAttachmentRepository0
        : Repository<GuestExtraFoodRequestAttachment>,
          IGuestExtraFoodRequestAttachmentRepository
    {
        public GuestExtraFoodRequestAttachmentRepository0(
            DbContext context)
            : base(context)
        {
        }

        public List<GuestExtraFoodRequestAttachment>
            GetByRequestId(long requestId)
        {
            return Entities
                .AsNoTracking()
                .Where(x =>
                    x.GuestExtraFoodRequestId ==
                        requestId &&
                    x.IsDeleted != true)
                .OrderByDescending(x =>
                    x.Id)
                .ToList();
        }
    }
}