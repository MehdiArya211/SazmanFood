using Domain.Entities;
using DTO.Base;
using DTO.Entities;
using Infrastructure.Data;

namespace BLL.Interface
{
    public interface IExtraQuotaPersonManager
        : IManager<GuestExtraFoodRequestPerson, ApplicationContext>
    {
        List<ExtraQuotaPersonListDTO> GetList(
            bool canViewAll,
            int? orgId);

        ExtraQuotaPersonFormDTO GetForm(
            long requestId,
            bool canViewAll);

        BaseResult CreateOfficial(
            ExtraQuotaOfficialPersonCreateDTO model,
            bool canViewAll);

        BaseResult CreateDuty(
            ExtraQuotaDutyPersonCreateDTO model,
            bool canViewAll);

        BaseResult DeletePerson(
            long id,
            bool canViewAll);
    }
}