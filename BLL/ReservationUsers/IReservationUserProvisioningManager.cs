using Domain.Entities;
using DTO.Base;

namespace BLL;

public interface IReservationUserProvisioningManager
{
    BaseResult EnsureReservationUser(
        long? personId,
        string personCode,
        string fullName,
        string nationalCode,
        long? organGarrisonId,
        long creatorId,
        int omdOrgId = 0);
}
