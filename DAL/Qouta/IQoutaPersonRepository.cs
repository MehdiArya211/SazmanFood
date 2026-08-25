using DAL.Interface;
using Domain.Entities;
using DTO.DataTable;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
	public interface IQoutaPersonRepository : IRepository<QoutaPerson>
	{
        public DataTableResponseDTO<QoutaPersonDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData,long userId ,long id);

    }
}
