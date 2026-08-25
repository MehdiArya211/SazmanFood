using BLL.Interface;
using Domain.Entities;
using Domain.Entities.Garrison;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using DTO.Entities.Garrison;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Garrision
{
    public interface IPersonManager : IManager<Person, ApplicationContext>
    {
        DataTableResponseDTO<PersonDTO> GetDataTableDTO(DataTableSearchDTO searchData, PersonDTO filters, long? organGarrisonId);
        BaseResult Create(PersonDTO model);
        bool ExistPerson(PersonDTO model);

    }
}
