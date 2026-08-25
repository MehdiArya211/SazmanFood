using Domain.Entities;
using DTO.DataTable;
using DTO.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.UnitStatisticDL
{
    public class UnitStatisticRepository
        : Repository<UnitStatistic>,
          IUnitStatisticRepository
    {
        public UnitStatisticRepository(
            DbContext context)
            : base(context)
        {
        }

        public DataTableResponseDTO<UnitStatisticDTO>
            GetDataTableDTO(
                DataTableSearchDTO searchData,
                UnitStatisticFilterDTO filters)
        {
            var query = Entities.AsNoTracking()
                .AsQueryable();

            if (filters.OrgId.HasValue)
            {
                query = query.Where(x =>
                    x.OrgId == filters.OrgId.Value);
            }

            if (filters.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == filters.Status.Value);
            }

            if (filters.IsActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive ==
                    filters.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(
                    searchData.searchValue))
            {
                var search =
                    searchData.searchValue.Trim();

                query = query.Where(x =>
                    x.OrgTitle.Contains(search) ||
                    x.CreatorFullName.Contains(search));
            }

            var totalCount =
                query.Count();

            if (searchData.sortDirection == "asc")
            {
                query = query.OrderBy(x => x.Id);
            }
            else
            {
                query = query.OrderByDescending(x => x.Id);
            }

            var data = query
                .Skip(searchData.start)
                .Take(searchData.length)
                .Select(UnitStatisticDTO.Selector)
                .ToList();

            return new DataTableResponseDTO<UnitStatisticDTO>
            {
                draw = searchData.draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = data
            };
        }
    }
}