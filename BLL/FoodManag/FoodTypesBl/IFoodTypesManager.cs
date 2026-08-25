using BLL.Interface;
using Domain.Entities.FoodManage;
using DTO.Base;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.FoodManag.FoodTypesBl
{
    public interface IFoodTypesManager : IManager<FoodTypes, ApplicationContext>
    {
        IList<SelectListDTO> GetAllFoodTypesListDTO();

    }
}
