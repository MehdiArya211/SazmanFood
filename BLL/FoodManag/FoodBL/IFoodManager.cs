using BLL.Interface;
using Domain.Entities.FoodManage;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities.FoodMang;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.FoodManag.FoodBL
{
    public interface IFoodManager : IManager<Foods, ApplicationContext>
    {
        DataTableResponseDTO<FoodDTO> GetDataTableDTO(DataTableSearchDTO searchData, FoodFilterDTO filters);
        BaseResult CreateFoods(FoodsCreateDTO creatfoods);

        FoodEditDTO GetFoodForEditDTO(long? id);
        BaseResult UpdateFoods(FoodEditDTO foodedit);
        List<FoodDTO> GetAllFood();

    }
}
