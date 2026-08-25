using DTO;
using DTO.DataTable;
using DTO.Entities.LogSystem;
using DTO.Entities.LogSystem.UserChangeLogEvent;
using Infrastructure.Data;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Z.EntityFramework.Plus;

namespace DAL
{
    public class SeriLogRepository : Repository<Domain.Entities.LogSystem.SeriLog>, ISeriLogRepository
    {
        public SeriLogRepository(DbContext _Context) : base(_Context)
        {
        }

        public DataTableResponseDTO<SeriLogDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, SeriLogFilter filters)
        {
            var model = new DataTableResponseDTO<SeriLogDataTableDTO>();

            var filter = PredicateBuilder.New<Domain.Entities.LogSystem.SeriLog>(true);

            #region شرط ها

            var recordTotal = Entities.DeferredCount(filter).FutureValue();

            //search
            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue;
                filter = filter.And(s => s.Level.Contains(srch));
            }
            #endregion

            #region فیلتر ها

            if (filters.Level != null)
            {
                filter = filter.And(x => x.Level == filters.Level);
            }

            //// تاریخ شروع
            if (filters.CreateStartDate != null)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0);
                filters.CreateStartDate = ((DateTime)filters.CreateStartDate).Date + ts;
                filter = filter.And(x => x.TimeStamp >= filters.CreateStartDate);
            }

            //// تاریخ پایان
            if (filters.CreateEndDate != null)
            {
                TimeSpan ts = new TimeSpan(23, 23, 23);
                filters.CreateEndDate = ((DateTime)filters.CreateEndDate).Date + ts;
                filter = filter.And(x => x.TimeStamp <= filters.CreateEndDate);
            }
            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            //sorting and paging
            var sortCol = searchData.sortColumnName;
            var selectedModel = Entities.Where(filter)
                                        .OrderBy(sortCol + " " + searchData.sortDirection)
                                        .Skip(searchData.start)
                                        .Take(searchData.length)
                                        .Select(SeriLogDataTableDTO.Selector)
                                        .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
        }
    }
}
