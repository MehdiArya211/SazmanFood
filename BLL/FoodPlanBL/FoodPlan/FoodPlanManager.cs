using BLL.Interface;
using Domain.Entities;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using DTO.User;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class FoodPlanManager : Manager<FoodPlan, ApplicationContext>, IFoodPlanManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession Session;

        public FoodPlanManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }



        /// <summary>
        /// گرفتن لیست کاربران برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<FoodPlanDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, FoodPlanFilterDataTableDTO filters)
        {
            return UOW.FoodPlan.GetDataTableDTO(searchData, filters);
        }

        public BaseResult Create(CreatFoodPlanDTO model)
        {
            var foodPlan = new FoodPlan()
            {
                YearsId = model.YearsId,
                SeasonId = model.SeasonId,
                OrganId = model.OrganId,
                OrganTitle = model.OrganTitle,
                IsActive = model.IsActive,
                RegUserId = model.UserCreateId,
                IsDeleted = false,
                CreateDate = DateTime.Now,
            };

            return base.Create(foodPlan);
        }

        public EditFoodPlanDTO GetEditDTO(long id)
        {
            if (id == null) return null;
            return UOW.FoodPlan.GetOneDTO<EditFoodPlanDTO>(EditFoodPlanDTO.Selector, x => x.Id == id);
        }
        public BaseResult Update(EditFoodPlanDTO model)
        {
            var foodPlan = UOW.FoodPlan.FirstOrDefault(x => x.Id == model.Id);
            foodPlan.YearsId = model.YearsId;
            foodPlan.SeasonId = model.SeasonId;
            foodPlan.OrganId = model.OrganId;
            foodPlan.OrganTitle = model.OrganTitle;
            foodPlan.IsActive = model.IsActive;
            foodPlan.LastEditUserId = model.UserCreateId;
            foodPlan.LastEditDate = DateTime.Now;
           

            return base.Update(foodPlan);
        }

        public string GetDayTitle(long dayId)
        {
            var day = UOW.Days.GetById(dayId);

            return day.Title;
        }

        public IList<SelectListDTO> GetSelectListYearsDTO()
        {
            return UOW.Years.GetDTO<SelectListDTO>(SelectListDTO.YearsSelector).ToList();
        }
        public IList<SelectListDTO> GetSelectListSeasonDTO()
        {
            return UOW.Season.GetDTO<SelectListDTO>(SelectListDTO.SeasonSelector).ToList();
        }
    }
}
