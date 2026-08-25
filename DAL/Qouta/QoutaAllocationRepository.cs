using Domain.Entities;
using DTO;
using DTO.DataTable;
using DTO.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Z.EntityFramework.Plus;

namespace DAL
{
    public class QoutaAllocationRepository : Repository<QoutaAllocation>, IQoutaAllocationRepository
    {
        public QoutaAllocationRepository(DbContext _Context) : base(_Context)
        {
        }
        public DataTableResponseDTO<QoutaAllocationDataTableDTO> GetDataTableDTO(
    DataTableSearchDTO searchData,
    QoutaAllocationUserAccessDTO userAccess,
    QoutaAllocationFilterDataTableDTO filters)
        {
            var model = new DataTableResponseDTO<QoutaAllocationDataTableDTO>();

            var recordTotal = Entities.DeferredCount().FutureValue();

            #region شرط ها

            var filter = PredicateBuilder.New<QoutaAllocation>(true);

            filter.And(x => x.IsDeleted == false);

            // کنترل نمایش بر اساس نقش کاربر
            if (userAccess != null && userAccess.IsAdmin)
            {
                // مدیر سیستم همه سهمیه‌ها را می‌بیند
            }
            else if (userAccess != null && userAccess.IsAdministrativeManager)
            {
                // مدیر اداری فقط سهمیه‌های یگان خودش را می‌بیند
                if (userAccess.OrganGarrisonId.HasValue)
                {
                    var organGarrisonId = userAccess.OrganGarrisonId.Value;
                    filter.And(x => x.OrganGarrisonId == organGarrisonId);
                }
                else
                {
                    // اگر مدیر اداری یگان نداشته باشد، چیزی نبیند
                    filter.And(x => x.IsDeleted == false);
                }
            }
            else
            {
                // سایر کاربران فقط سهمیه‌هایی که خودشان ثبت کرده‌اند
                var userId = userAccess?.UserId ?? 0;
                filter.And(x => x.RegUserId == userId);
            }

            if (filters != null)
            {
                if (filters.OrganGarrisonId != null)
                    filter.And(x => x.OrganGarrisonId == filters.OrganGarrisonId);

                if (filters.DayId != null)
                    filter.And(x => x.DayId == filters.DayId);

                if (filters.MealId != null)
                    filter.And(x => x.MealId == filters.MealId);

            }

            // search
            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue;

                filter.And(x =>
                    x.DayTitle.Contains(srch) ||
                    x.MealTitle.Contains(srch) ||
                    x.OrgTitle.Contains(srch) ||
                    x.OrganGarrison.Title.Contains(srch));
            }

            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            // sorting and paging
            var sortCol = searchData.sortColumnName;

            var selectedModel = Entities
                .Include(x => x.OrganGarrison).ThenInclude(g => g.Parent)
                .Include(x => x.Day)
                .Include(x => x.Meal)
                .Include(x => x.FoodPlanDay).ThenInclude(f => f.Food)
                .Include(x => x.QoutaPerson).ThenInclude(p => p.MainFood)
                .Where(filter)
                .OrderBy(sortCol + " " + searchData.sortDirection)
                .Skip(searchData.start)
                .Take(searchData.length)
                .Select(QoutaAllocationDataTableDTO.Selector)
                .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;

            return model;
        }
    }
}
