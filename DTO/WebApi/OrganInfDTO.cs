using DTO.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class OrganInfDTO : EntityDTO
    {
        

        public string Title { get; set; }
        public string UnitTitle { get; set; }
       

    

    }
    public class ApiResultOrganDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<OrganViewModelDto> Data { get; set; }
    }

    public class OrganViewModelDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UnitTitle { get; set; }


    }
    public class OrganInfoViewModel
    {
        public int Id { get; set; }
        public string UnitTitle { get; set; }
        public string Title { get; set; }

    }
}
