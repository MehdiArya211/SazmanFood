using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class DbChangeLogEvent
    {
        public long Id { get; set; }

        public ChangeEventType ChangeEventType { get; set; }

        [StringLength(15, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string ChangeEventTypeDesctiption { get; set; }

        [StringLength(128, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string TableName { get; set; }

        public long? TableId { get; set; }

        [StringLength(1000, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string NewData { get; set; }

        [StringLength(1000, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string OldData { get; set; }

        public long UserId { get; set; }

        public int? UserOrgId { get; set; }

        [StringLength(50, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string UserFullName { get; set; }

        [StringLength(30, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string UserName { get; set; }

        [StringLength(15, ErrorMessage = "{0} حداکثر میتواند {1} کاراکتر باشد!")]
        public string IpAddress { get; set; }

        public DateTime LogDateTime { get; set; } = DateTime.Now;
    }

    public enum ChangeEventType
    {
        Detached,

        Unchanged,

        // حذف
        Deleted,

        // ویرایش
        Modified,

        // ایجاد
        Added
    }
}
