using DAL.Interface;
using Domain.Entities;
using Domain.Entities.Garrison;
using DTO.DataTable;
using DTO.Entities.Garrison;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Garrison
{
    public interface IPersonRepository : IRepository<Person>
    {
        DataTableResponseDTO<PersonDTO> GetDataTableDTO(DataTableSearchDTO searchData, PersonDTO filters, long? organGarrisonId);

    }
}
