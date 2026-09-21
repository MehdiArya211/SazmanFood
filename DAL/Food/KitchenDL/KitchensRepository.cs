using DAL.Interface;
using Domain.Entities.FoodManage;
using DTO.DataTable;
using DTO.Entities.FoodMang;
using DTO.Entities.Kitchen;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.EntityFramework.Plus;
using System.Linq.Dynamic.Core;

namespace DAL.Food.KitchenDL
{
    public class KitchensRepository : Repository<Kitchens>, IKitchensRepository
    {
        public KitchensRepository(DbContext _Context):base(_Context)  
        {

        }

        public DataTableResponseDTO<KitchenDTO> GetDataTableDTO(DataTableSearchDTO searchData, KitchenFilterDTO filters)
        {
            var model = new DataTableResponseDTO<KitchenDTO>();

            var recordTotal = Entities.DeferredCount().FutureValue();

            #region شرط ها

            var filter = PredicateBuilder.New<Kitchens>(true).And(d => d.IsDeleted == false);



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
                                        .Select(KitchenDTO.Selector)
                                        .Future();

            model.data = selectedModel.ToList();
            model.recordsTotal = recordTotal.Value;
            model.recordsFiltered = recordsFiltered.Value;
            model.draw = searchData.draw;
            return model;
        }

        /// <summary>
        /// دریافت آمار پخت آشپزخانه‌ها بر اساس سهمیه و نفرات ثبت‌شده
        /// </summary>
        public KitchenCookingStatisticsDTO GetCookingStatistics(
            DateTime fromDate,
            DateTime toDate,
            long? kitchenId,
            long? mealId)
        {
            fromDate = fromDate.Date;
            toDate = toDate.Date;
            var endDate = toDate.AddDays(1);

            var rows = (
                from kitchen in Entities.AsNoTracking()
                join allocation in Context.Set<QoutaAllocation>().AsNoTracking()
                    on (long?)kitchen.OrgId equals allocation.OrgId
                where kitchen.IsDeleted != true &&
                      kitchen.IsActive == true &&
                      allocation.IsDeleted != true &&
                      allocation.QoutaAllocationDate >= fromDate &&
                      allocation.QoutaAllocationDate < endDate &&
                      (!kitchenId.HasValue || kitchen.Id == kitchenId.Value) &&
                      (!mealId.HasValue || allocation.MealId == mealId.Value)
                group allocation by new
                {
                    kitchen.Id,
                    KitchenTitle = kitchen.Title,
                    kitchen.OrgTitle,
                    CookingCapacity = kitchen.CookingCapacity ?? 0,
                    Date = allocation.QoutaAllocationDate.Date,
                    allocation.MealId,
                    allocation.MealTitle
                }
                into grouped
                orderby grouped.Key.Date,
                        grouped.Key.KitchenTitle,
                        grouped.Key.MealTitle
                select new KitchenCookingStatisticsRowDTO
                {
                    KitchenId = grouped.Key.Id,
                    KitchenTitle = grouped.Key.KitchenTitle,
                    OrgTitle = grouped.Key.OrgTitle,
                    Date = grouped.Key.Date,
                    MealId = grouped.Key.MealId,
                    MealTitle = grouped.Key.MealTitle ?? "-",
                    CookingCapacity = grouped.Key.CookingCapacity,
                    QuotaCount = grouped.Sum(x =>
                        x.OfficerCapacity +
                        x.SoldierCapacity +
                        x.GuestCapacity +
                        x.ManagementTokenCapacity),
                    ReservedCount = grouped.Sum(x =>
                        x.QoutaPerson.Count(p => p.IsDeleted != true)),
                    DeliveredCount = grouped.Sum(x =>
                        x.QoutaPerson.Count(p =>
                            p.IsDeleted != true &&
                            p.IsDelivered == true))
                }).ToList();

            var kitchens = Entities
                .AsNoTracking()
                .Where(x => x.IsDeleted != true && x.IsActive == true)
                .OrderBy(x => x.OrgTitle)
                .ThenBy(x => x.Title)
                .Select(x => new KitchenCookingFilterDTO
                {
                    Id = x.Id,
                    Title = x.Title + " - " + x.OrgTitle
                })
                .ToList();

            var meals = Context.Set<Meal>()
                .AsNoTracking()
                .OrderBy(x => x.SortName)
                .ThenBy(x => x.Title)
                .Select(x => new KitchenCookingFilterDTO
                {
                    Id = x.Id,
                    Title = x.Title
                })
                .ToList();

            return new KitchenCookingStatisticsDTO
            {
                FromDate = fromDate,
                ToDate = toDate,
                KitchenId = kitchenId,
                MealId = mealId,
                Kitchens = kitchens,
                Meals = meals,
                Rows = rows
            };
        }
    }
}
