using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTO.UserLog
{
    public class UserActionLogFilter
    {


        [Display(Name = "نام کاربر")]
        public string FullName { get; set; }


        [Display(Name = "نوع فعالیت")]
        public ActionType? ActionType { get; set; }


        [Display(Name = "حوزه")]
        public MenuType? MenuType { get; set; }


        [Display(Name = "توضیحات")]
        public string Description { get; set; }


        [Display(Name = "وضعیت ورود")]
        public bool? IsSuccess { get; set; }


        [Display(Name = "ایجاد از تاریخ")]
        public DateTime? CreateStartDate { get; set; }


        [Display(Name = "ایجاد تا تاریخ")]
        public DateTime? CreateEndDate { get; set; }
    }
}
