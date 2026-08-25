using DAL.Interface;
using Domain.Entities;
using DTO.DataTable;
using DTO.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
using System.Linq.Dynamic.Core;

namespace DAL
{
    public class FoodSourceRepository : Repository<FoodSource>, IFoodSourceRepository
    {
        public FoodSourceRepository(DbContext _Context) : base(_Context)
        {
        }


        /// <summary>
        /// گرفتن لیست کاربران برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<FoodSourceDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, FoodSourceFilterDataTableDTO filters)
        {
            var model = new DataTableResponseDTO<FoodSourceDataTableDTO>();

            var recordTotal = Entities.DeferredCount().FutureValue();

            #region شرط ها

            var filter = PredicateBuilder.New<FoodSource>(true).And(d => d.IsDeleted == false);



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
                filter.And(s => s.Id.ToString().Contains(srch));
            }


            #endregion

            var recordsFiltered = Entities.DeferredCount(filter).FutureValue();

            //sorting and paging
            var sortCol = searchData.sortColumnName;
            var selectedModel = Entities.Where(filter)
                                        .OrderBy(sortCol + " " + searchData.sortDirection)
                                        .Skip(searchData.start)
                                        .Take(searchData.length)
                                        .Select(FoodSourceDataTableDTO.Selector)
                                        .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
        }


    }
}
