using Domain.Enums.MaxaRabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entities.MaxaRabbitMQ
{
    /// <summary>
    /// اطلاعاتی که پس از احراز هویت ویردی به ما برمیگردونه تا پردازش های خودمون رو انجام بدیم
    /// </summary>


    public class AccessLogMonitoringEvent
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public uint RelayActionInputPort { get; set; }
        public uint RelayActionRelayPort { get; set; }
        public string AuthMethod { get; set; }
        public string Status { get; set; }
        public string Where { get; set; }
        public string DeviceName { get; set; }
        public uint DoorId { get; set; }
        public uint ZoneId { get; set; }
        public string IOInfo { get; set; }
        public string TnaKeyStr { get; set; }
        public bool Image { get; set; }
        public DeviceInOutMode InOutMode { get; set; }
        public UInt32 EventId { get; set; }
        public DateTime EventDateTime { get; set; }
        public string DeviceModelName { get; set; }
        public uint DeviceId { get; set; }
        public int EventType { get; set; }
        public long UserId { get; set; }
        public string UniqueID { get; set; }
        public long EventLogId { get; set; }
        public Guid BrandId { get; set; }
        public byte[] LogImageThumb { get; set; }
        public string Type { get; set; }

        public int Version { get; set; }
    }

}
