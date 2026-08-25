using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class SeriLogFilter
    {
        [Display(Name = "نوع")]
        public string Level { get; set; }

        [Display(Name = "ایجاد از تاریخ")]
        public DateTime? CreateStartDate { get; set; }

        [Display(Name = "ایجاد تا تاریخ")]
        public DateTime? CreateEndDate { get; set; }
    }
}
