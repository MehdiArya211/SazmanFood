using BLL.Interface;
using Domain.Entities;
using DTO.Base;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MealManager : Manager<Meal, ApplicationContext>, IMealManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession Session;

        public MealManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }

        public IList<SelectListDTO> GetSelectListDTO()
        {
                return UOW.Meal.GetDTO<SelectListDTO>(SelectListDTO.MealSelector).ToList();
        }
    }
}
