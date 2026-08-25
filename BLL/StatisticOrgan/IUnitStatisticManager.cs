using Domain.Entities;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using Infrastructure.Data;

namespace BLL.Interface
{
    public interface IUnitStatisticManager :
        IManager<UnitStatistic, ApplicationContext>
    {
        DataTableResponseDTO<UnitStatisticDTO> GetDataTableDTO(
            DataTableSearchDTO searchData,
            UnitStatisticFilterDTO filters);

        BaseResult Create(
            UnitStatisticCreateDTO model,
            long creatorId,
            string creatorFullName);

        UnitStatisticEditDTO GetEditDTO(long id);

        BaseResult Update(UnitStatisticEditDTO model);

        UnitStatisticDetailsDTO GetDetailsDTO(long? id);

        BaseResult SaveDetails(UnitStatisticDetailsDTO model);

        BaseResult Send(long id);

        BaseResult Approve(long id);

        /// <summary>
        /// عودت آمار ارسال‌شده برای اصلاح
        /// </summary>
        BaseResult Return(long id, string reason);

        /// <summary>
        /// لغو آمار یگان
        /// </summary>
        BaseResult Cancel(long id, string reason);
    }
}