using BLL.Interface;
using Domain.Entities;
using DTO.DataTable;
using DTO;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Base;
using DTO.User;
using DTO.Entities;

namespace BLL
{
	public interface IQoutaAllocationManager : IManager<QoutaAllocation, ApplicationContext>
	{
        public DataTableResponseDTO<QoutaAllocationDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, QoutaAllocationFilterDataTableDTO filters);

        IList<SelectListDTO> GetSelectListDTO();

        BaseResult Create(QoutaAllocationCreateDTO model);

        QoutaAllocationEditDTO GetEditDTO(long? id);

        BaseResult Update(QoutaAllocationEditDTO model);

    }
}
