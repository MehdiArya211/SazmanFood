using Domain.Entities;
using DTO;
using DTO.DataTable;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;
using System.Linq.Dynamic.Core;

namespace DAL
{
    public class QoutaPersonRepository : Repository<QoutaPerson>, IQoutaPersonRepository
    {
        public QoutaPersonRepository(DbContext _Context) : base(_Context)
        {
        }

        public DataTableResponseDTO<QoutaPersonDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, long userId, long id)
        {
            var model = new DataTableResponseDTO<QoutaPersonDataTableDTO>();

            var recordTotal = Entities.DeferredCount().FutureValue();

            #region شرط ها

            var filter = PredicateBuilder.New<QoutaPerson>(true);

            filter.And(x => x.RegUserId == userId);
            filter.And(x => x.QoutaAllocationId == id);

            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            //sorting and paging
            var sortCol = searchData.sortColumnName;
            var selectedModel = Entities.Where(filter)
                                        .OrderBy(sortCol + " " + searchData.sortDirection)
                                        .Skip(searchData.start)
                                        .Take(searchData.length)
                                        .Select(QoutaPersonDataTableDTO.Selector)
                                        .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
        }


    }
}
