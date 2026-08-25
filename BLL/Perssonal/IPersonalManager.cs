using BLL.Interface;
using Domain.Entities;
using DTO;
using DTO.Base;
using DTO.DataTable;
using Infrastructure.Data;

namespace BLL
{
    public interface IPersonalManager : IManager<PersonalType, ApplicationContext>
    {
        IList<SelectListDTO> GetSelectListDTO();



    }
}
