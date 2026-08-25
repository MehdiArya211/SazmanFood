using DAL.Interface;
using Domain.Entities;
using Domain.Entities.LogSystem;
using DTO.DataTable;
using DTO.Entities.LogSystem.UserChangeLogEvent;
using Infrastructure.Data;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Z.EntityFramework.Plus;

namespace DAL.LogSystem.UserChangeLogEvent
{
    public class UserChangeLogEventRepository : Repository<TrackDatabaseLog>, IUserChangeLogEventRepository
    {
        public UserChangeLogEventRepository(LogContext _Context) : base(_Context)
        {
        }

        public DataTableResponseDTO<UserChagneLogEventDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserChagneLogEventFilter filters)
        {
            var model = new DataTableResponseDTO<UserChagneLogEventDataTableDTO>();

            var filter = PredicateBuilder.New<TrackDatabaseLog>(true);

            #region شرط ها

            var recordTotal = Entities.DeferredCount(filter).FutureValue();

            //search
            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue;
                filter = filter.And(s => s.TableName.Contains(srch));
            }
            #endregion

            #region فیلتر ها


            //// تاریخ شروع
            //if (filters.CreateStartDate != null)
            //{
            //    TimeSpan ts = new TimeSpan(0, 0, 0);
            //    filters.CreateStartDate = ((DateTime)filters.CreateStartDate).Date + ts;
            //    filter = filter.And(x => x.CreateDate >= filters.CreateStartDate);
            //}

            //// تاریخ پایان
            //if (filters.CreateEndDate != null)
            //{
            //    TimeSpan ts = new TimeSpan(23, 23, 23);
            //    filters.CreateEndDate = ((DateTime)filters.CreateEndDate).Date + ts;
            //    filter = filter.And(x => x.CreateDate <= filters.CreateEndDate);
            //}
            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            //sorting and paging
            var sortCol = searchData.sortColumnName;
            var selectedModel = Entities.Where(filter)
                                        .OrderBy(sortCol + " " + searchData.sortDirection)
                                        .Skip(searchData.start)
                                        .Take(searchData.length)
                                        .Select(UserChagneLogEventDataTableDTO.Selector)
                                        .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
        }

        public UserChagneLogEventDTO GetDetailDTO(long id)
        {
            return Entities.Where(wh => wh.Id == id).Select(UserChagneLogEventDTO.Selector).First();
        }
    }
}
