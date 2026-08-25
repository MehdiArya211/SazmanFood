using Domain.Entities.LogSystem;
using DTO;
using DTO.DataTable;
using DTO.Entities.LogSystem;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class SeriLogManager : Manager<SeriLog, ApplicationContext>, ISeriLogManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;


        public SeriLogManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public DataTableResponseDTO<SeriLogDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, SeriLogFilter filters)
        {
            return UOW.SeriLogRepository.GetDataTableDTO(searchData , filters);
        }
    }
}
