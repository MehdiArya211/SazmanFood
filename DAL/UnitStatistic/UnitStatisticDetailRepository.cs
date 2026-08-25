using Domain.Entities;
using DTO.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class UnitStatisticDetailRepository
    : Repository<UnitStatisticDetail>,
      IUnitStatisticDetailRepository
{
    public UnitStatisticDetailRepository(
        DbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// دریافت جزئیات آمار بر اساس نوع پرسنل
    /// </summary>
    public List<UnitStatisticDetailItemDTO> GetDetails(
        long unitStatisticId,
        long personalTypeId)
    {
        return Entities
            .AsNoTracking()
            .Where(x =>
                x.UnitStatisticId ==
                unitStatisticId &&
                x.PersonalTypeId ==
                personalTypeId)
            .OrderBy(x =>
                x.YeganType.SortName)
            .Select(
                UnitStatisticDetailItemDTO.Selector)
            .ToList();
    }

    /// <summary>
    /// دریافت تمام جزئیات یک آمار
    /// </summary>
    public List<UnitStatisticDetail>
        GetByUnitStatisticId(
            long unitStatisticId)
    {
        return Entities
            .Where(x =>
                x.UnitStatisticId ==
                unitStatisticId)
            .ToList();
    }

    /// <summary>
    /// حذف تمام جزئیات مربوط به یک آمار
    /// </summary>
    public void RemoveByUnitStatisticId(
        long unitStatisticId)
    {
        var details = Entities
            .Where(x =>
                x.UnitStatisticId ==
                unitStatisticId)
            .ToList();

        if (details.Any())
        {
            Entities.RemoveRange(details);
        }
    }

    /// <summary>
    /// محاسبه مجموع تعداد بر اساس نوع پرسنل
    /// </summary>
    public int GetTotalCount(
        long unitStatisticId,
        long personalTypeId)
    {
        return Entities
            .Where(x =>
                x.UnitStatisticId ==
                unitStatisticId &&
                x.PersonalTypeId ==
                personalTypeId)
            .Sum(x => (int?)x.Count) ?? 0;
    }
}