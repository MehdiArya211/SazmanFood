using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entities.MaxaRabbitMQ
{
    public class DeviceLockMessageDTO
    {
        public long TerminalId { get; set; }
        public bool IsDeviceLock { get; set; }

    }
}
