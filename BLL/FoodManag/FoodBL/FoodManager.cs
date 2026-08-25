using Domain.Entities.FoodManage;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using DTO.Entities.FoodMang;
using DTO.User;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.FoodManag.FoodBL
{
    public class FoodManager : Manager<Foods, ApplicationContext>, IFoodManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession Session;
        public FoodManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }

        public BaseResult CreateFoods(FoodsCreateDTO creatfoods)
        {
            var opsystem = new Foods()
            {
                Title = creatfoods.Title,
                Code = creatfoods.Code,
                FoodTypesId = creatfoods.FoodTypesId,

            };

            return base.Create(opsystem);
        }

        public List<FoodDTO> GetAllFood()
        {
            var model = UOW.Food.GetDTO<FoodDTO>(FoodDTO.Selector, null, null, null, null).ToList();
            return model;
        }

        public DataTableResponseDTO<FoodDTO> GetDataTableDTO(DataTableSearchDTO searchData, FoodFilterDTO filters)
        {
            return UOW.Food.GetDataTableDTO(searchData, filters);
        }

        public FoodEditDTO GetFoodForEditDTO(long? id)
        {
            if (id == null) return null;
            return UOW.Food.GetOneDTO<FoodEditDTO>(FoodEditDTO.Selector, x => x.Id == id);
        }

        public BaseResult UpdateFoods(FoodEditDTO foodedit)
        {
            var per = UOW.Food.FirstOrDefault(x => x.Id == foodedit.Id);

            per.Title = foodedit.Title;
            per.FoodTypesId = foodedit.FoodTypesId;
            per.Code = foodedit.Code;



            return base.Update(per);
        }
    }
}
