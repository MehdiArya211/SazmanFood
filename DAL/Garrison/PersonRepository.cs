using Domain.Entities.Garrison;
using DTO.DataTable;
using DTO.Entities.Garrison;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Utilities.Extentions;

namespace DAL.Garrison
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(DbContext _Context) : base(_Context)
        {

        }

        public DataTableResponseDTO<PersonDTO> GetDataTableDTO(DataTableSearchDTO searchData, PersonDTO filters,long? organGarrisonId)
        {
            var model = new DataTableResponseDTO<PersonDTO>();

            model.recordsTotal = Count();

            var filter = PredicateBuilder.New<Person>(true);
            filter.And(m => m.OrganGarrisonId == organGarrisonId);


            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue.ToEnglishNumber();
                filter = filter.And(s => s.FullName.Contains(srch));
            }

            var selectedModel = GetDTO<PersonDTO>(PersonDTO.Selector, filter);

            model.recordsFiltered = selectedModel.Count();

            //sorting
            var sortCol = searchData.sortColumnName;
            selectedModel = selectedModel.AsQueryable().OrderBy(sortCol + " " + searchData.sortDirection);

            //paging
            model.data = selectedModel.Skip(searchData.start).Take(searchData.length).ToList();
            model.draw = searchData.draw;
            return model;
        }
    }
}
