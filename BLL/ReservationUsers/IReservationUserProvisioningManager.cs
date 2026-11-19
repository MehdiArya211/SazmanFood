using Domain.Entities;
using DTO.Base;

namespace BLL;

public interface IReservationUserProvisioningManager
{
    BaseResult EnsureReservationRoleIfMissing(long userId);

    BaseResult EnsureReservationUser(
        long? personId,
        string personCode,
        string fullName,
        string nationalCode,
        long? organGarrisonId,
        long creatorId,
        int omdOrgId = 0);
}
