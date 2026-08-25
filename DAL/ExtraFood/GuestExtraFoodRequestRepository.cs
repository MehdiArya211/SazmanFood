using DAL.Interface;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class GuestExtraFoodRequestRepository
        : Repository<GuestExtraFoodRequest>,
          IGuestExtraFoodRequestRepository
    {
        public GuestExtraFoodRequestRepository(
            DbContext context)
            : base(context)
        {
        }

        public GuestExtraFoodRequest GetRequest(long id)
        {
            return Entities.FirstOrDefault(x =>
                x.Id == id &&
                x.IsDeleted != true);
        }

        public int GetApprovedExtraQuota(
            int orgId,
            long mealId,
            long personalTypeId,
            long yeganTypeId,
            DateTime date)
        {
            var targetDate = date.Date;

            return Entities
                .Where(x =>
                    x.OrgId == orgId &&
                    x.MealId == mealId &&
                    x.PersonalTypeId == personalTypeId &&
                    x.YeganTypeId == yeganTypeId &&
                    x.Status ==
                        GuestExtraFoodRequestStatus.Approved &&
                    x.FromDate <= targetDate &&
                    x.ToDate >= targetDate &&
                    x.IsDeleted != true)
                .Sum(x => (int?)x.ExtraQuotaCount) ?? 0;
        }
    }

    public class GuestExtraFoodRequestAttachmentRepository
        : Repository<GuestExtraFoodRequestAttachment>,
          IGuestExtraFoodRequestAttachmentRepository
    {
        public GuestExtraFoodRequestAttachmentRepository(
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
                    x.GuestExtraFoodRequestId == requestId &&
                    x.IsDeleted != true)
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}