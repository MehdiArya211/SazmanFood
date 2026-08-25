using Domain.Entities.FoodManage;
using DTO.Base;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.FoodManag.FoodTypesBl
{
    public class FoodTypesManager : Manager<FoodTypes, ApplicationContext>, IFoodTypesManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession Session;
        public FoodTypesManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }

        public IList<SelectListDTO> GetAllFoodTypesListDTO()
        {
            return UOW.FoodTypes.GetDTO<SelectListDTO>(SelectListDTO.FoodTypesSelector, x => x.IsDeleted == false).ToList();
        }
    }
}
