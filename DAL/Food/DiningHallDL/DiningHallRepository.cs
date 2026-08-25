using DAL.Interface;
using Domain.Entities.FoodManage;
using DTO.DataTable;
using DTO.Entities.DiningHalDTo;
using DTO.Entities.FoodMang;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.EntityFramework.Plus;
using System.Linq.Dynamic.Core;
using DTO.Entities;

namespace DAL
{
    public class DiningHallRepository : Repository<DiningHall>, IDiningHallRepository
    {
        public DiningHallRepository(DbContext _Context) : base(_Context)
        {

        }

        public DataTableResponseDTO<DiningHallDTO> GetDataTableDTO(DataTableSearchDTO searchData, DiningHallFilterDTO filters)
        {
            var model = new DataTableResponseDTO<DiningHallDTO>();

            var recordTotal = Entities.DeferredCount().FutureValue();

            #region شرط ها

            var filter = PredicateBuilder.New<DiningHall>(true).And(d => d.IsDeleted == false);



            //// نام کاربری
            //if (!string.IsNullOrEmpty(filters.Username))
            //    filter.And(x => x.Username.Contains(filters.Username));

            ////فعال
            //if (filters.IsEnabled != null)
            //    filter.And(x => x.IsEnabled == filters.IsEnabled);


            //search
            if (!string.IsNullOrEmpty(searchData.searchValue))
            {
                var srch = searchData.searchValue;
                filter.And(s => s.Title.Contains(srch));
            }


            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            //sorting and paging
            var sortCol = searchData.sortColumnName;
            var selectedModel = Entities.Where(filter)
                                        .OrderBy(sortCol + " " + searchData.sortDirection)
                                        .Skip(searchData.start)
                                        .Take(searchData.length)
                                        .Select(DiningHallDTO.Selector)
                                        .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
        }
    }
}
