using DTO.DataTable;
using DTO.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;
using System.Linq.Dynamic.Core;
using Myrmec;
using System.Numerics;
using Utilities.Extentions;
using Domain.Entities.Garrison;
using DTO;

namespace DAL
{
    public class OrganGarrisonRepository : Repository<OrganGarrison>, IOrganGarrisonRepository
	{
		public OrganGarrisonRepository(DbContext _Context) : base(_Context)
		{
		}

        public DataTableResponseDTO<OrganGarrisonDTO> GetDataTableDTO(DataTableSearchDTO searchData, OrganGarrisonDTO filters)
        {
            var model = new DataTableResponseDTO<OrganGarrisonDTO>();

            model.recordsTotal = Count();

            var filter = PredicateBuilder.New<OrganGarrison>(true);
           

            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue.ToEnglishNumber();
                filter = filter.And(s =>  s.Title.Contains(srch));
            }

			var selectedModel = GetDTO<OrganGarrisonDataTableDTO>(OrganGarrisonDataTableDTO.Selector, filter)
				.Where(m => m.IsDeleted == false);

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
