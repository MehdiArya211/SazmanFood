using Domain.Entities;
using DTO.Base;
using DTO.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace BLL.Interface;

public interface IGuestExtraFoodRequestManager
    : IManager<GuestExtraFoodRequest, ApplicationContext>
{       

    bool CanView();
    bool CanViewAllOrganizations();
    bool CanApprove();
    bool CanPrintGuest();
    int? GetCurrentUserOrganizationId();


    #region Lookup data

    IList<SelectListDTO> GetOrganizationSelectList();
    IList<SelectListDTO> GetMealSelectList();
    IList<SelectListDTO> GetPersonalTypeSelectList();
    IList<SelectListDTO> GetYeganTypeSelectList();

    #endregion

    #region Requests

    List<GuestExtraFoodRequestDTO> GetList(GuestExtraFoodRequestFilterDTO filters);
    GuestExtraFoodRequestEditDTO GetEditDTO(long id);
    BaseResult CreateRequest(GuestExtraFoodRequestCreateDTO model);
    BaseResult EditRequest(GuestExtraFoodRequestEditDTO model);
    BaseResult DeleteRequest(long id);
    BaseResult Send(long id);
    BaseResult Approve(long id);

    #endregion

    #region Attachments

    List<GuestExtraFoodRequestAttachmentDTO> GetAttachments(long requestId);
    BaseResult AddAttachment(long requestId, IFormFile file, string description);
    BaseResult DeleteAttachment(long id);
    GuestExtraFoodRequestAttachment GetAttachment(long id);

    #endregion

    GuestExtraFoodRequest GetForPrint(long id);
}