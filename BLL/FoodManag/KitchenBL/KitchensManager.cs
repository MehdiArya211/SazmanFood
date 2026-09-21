using BLL.FoodManag.KitchenBL;
using Domain.Entities.FoodManage;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities.FoodMang;
using DTO.Entities.Kitchen;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.FoodManag.KitchenBL
{
    public class KitchensManager : Manager<Kitchens, ApplicationContext>, IKitchensManager
    {
        public KitchensManager(
            DbContexts contexts,
            IHttpContextAccessor httpContextAccessor)
            : base(contexts, httpContextAccessor)
        {
        }

        public BaseResult CreateKitchens(KitchenCreateDTO creatkitchen)
        {
            var opsystem = new Kitchens()
            {
                Title = creatkitchen.Title,
                CookingCapacity = creatkitchen.CookingCapacity,
                IsActive = creatkitchen.IsActive,
                OrgId = creatkitchen.OrgId,
                OrgTitle = creatkitchen.OrgTitle,
                Description = creatkitchen.Description,

            };
            return base.Create(opsystem);
        }

        public DataTableResponseDTO<KitchenDTO> GetDataTableDTO(DataTableSearchDTO searchData, KitchenFilterDTO filters)
        {
            return UOW.kitchen.GetDataTableDTO(searchData, filters);
        }

        public KitchenEditDTO GetKitchenForEditDTO(long? id)
        {
            if (id == null) return null;
            return UOW.kitchen.GetOneDTO<KitchenEditDTO>(KitchenEditDTO.Selector, x => x.Id == id);
        }

        public BaseResult UpdateKitchens(KitchenEditDTO kitchenedit)
        {
            var per = UOW.kitchen.FirstOrDefault(x => x.Id == kitchenedit.Id);

            per.Title = kitchenedit.Title;
            per.CookingCapacity = kitchenedit.CookingCapacity;
            per.OrgTitle = kitchenedit.OrgTitle;
            per.OrgId = kitchenedit.OrgId;
            per.IsActive = kitchenedit.IsActive;
            per.Description = kitchenedit.Description;
            return base.Update(per);
        }

        public KitchenCookingStatisticsDTO GetCookingStatistics(
            DateTime fromDate,
            DateTime toDate,
            long? kitchenId,
            long? mealId)
        {
            return UOW.kitchen.GetCookingStatistics(
                fromDate,
                toDate,
                kitchenId,
                mealId);
        }

    }
}
