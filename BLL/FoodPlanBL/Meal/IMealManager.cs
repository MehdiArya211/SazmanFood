using BLL.Interface;
using Domain.Entities;
using DTO.Base;
using Infrastructure.Data;

namespace BLL
{
    public interface IMealManager : IManager<Meal, ApplicationContext>
    {
        IList<SelectListDTO> GetSelectListDTO();



    }
}
