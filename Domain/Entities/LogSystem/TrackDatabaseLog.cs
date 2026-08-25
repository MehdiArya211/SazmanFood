using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities.LogSystem
{
    public sealed class TrackDatabaseLog
    {
        public long Id { get; set; }

        public TrackEventType TrackEventType { get; set; }

        [StringLength(100, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string TrackEventTypeDescription { get; set; }

        [StringLength(200, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string TableName { get; set; }

        public long? TableId { get; set; }

        [StringLength(4000, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string NewData { get; set; }

        [StringLength(4000, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string OldData { get; set; }

        public long UserId { get; set; }

        public int? UserUnitId { get; set; }

        [StringLength(200, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string UserFullName { get; set; }

        [StringLength(200, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string UserName { get; set; }

        [StringLength(20, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string IpAddress { get; set; }

        public DateTime LogDateTime { get; set; } = DateTime.Now;
    }
}
