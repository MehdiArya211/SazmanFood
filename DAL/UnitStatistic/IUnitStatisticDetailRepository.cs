using DAL.Interface;
using Domain.Entities;
using DTO.Entities;

namespace DAL;

public interface IUnitStatisticDetailRepository
    : IRepository<UnitStatisticDetail>
{
    /// <summary>
    /// دریافت جزئیات آمار بر اساس شناسه آمار
    /// </summary>
    List<UnitStatisticDetailItemDTO> GetDetails(
        long unitStatisticId,
        long personalTypeId);

    /// <summary>
    /// دریافت تمام جزئیات یک آمار
    /// </summary>
    List<UnitStatisticDetail> GetByUnitStatisticId(
        long unitStatisticId);

    /// <summary>
    /// حذف تمام جزئیات یک آمار
    /// </summary>
    void RemoveByUnitStatisticId(
        long unitStatisticId);

    /// <summary>
    /// محاسبه مجموع تعداد بر اساس نوع پرسنل
    /// </summary>
    int GetTotalCount(
        long unitStatisticId,
        long personalTypeId);
}