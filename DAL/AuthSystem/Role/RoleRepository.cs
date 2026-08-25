using DAL.Interface;
using Domain.Entities;
using DTO.DataTable;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace DAL
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(DbContext _Context) : base(_Context)
        {
        }



        /// <summary>
        /// گرفتن لیست کاربران برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<Role> GetDataTableDTO(DataTableSearchDTO searchData)
        {
            var model = new DataTableResponseDTO<Role>();

            model.recordsTotal = Count();

            var filter = PredicateBuilder.New<Role>(true);

            //search
            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue;
                filter = filter.And(s => s.Title.Contains(srch)
                                              || s.Description.Contains(srch)
                                              || s.Id.ToString().Contains(srch));
            }

            var selectedModel = Entities.Where(filter);
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