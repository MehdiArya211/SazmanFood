using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entities
{
    public class DimaDetectedMessageDTO
    {
        public string EmployeeId { get; set; }
        public DateTime Timestamp { get; set; }
        public string DeviceId { get; set; }
        public string TemplateType { get; set; }
    }

}
