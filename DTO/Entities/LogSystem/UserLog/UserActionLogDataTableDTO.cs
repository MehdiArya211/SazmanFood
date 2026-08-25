using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Utilities.Extentions;

namespace DTO.UserLog
{
    public class UserActionLogDataTableDTO
    {
        [Display(Name = "شناسه")]
        public long Id { get; set; }


        [Display(Name = "نام کاربر")]
        public string UserName { get; set; }


        [Display(Name = "آی پی")]
        public string UserIp { get; set; }


        [Display(Name = "نوع فعالیت")]
        public string ActionName { get; set; }


        [Display(Name = "حوزه")]
        public string MenuName { get; set; }


        [Display(Name = "شناسه موجودیت")]
        public long? TargetId { get; set; }


        [Display(Name = "توضیحات")]
        public string Description { get; set; }


        [Display(Name = "وضعیت انجام")]
        public bool IsSuccess { get; set; }
        [Display(Name = "وضعیت انجام")]
        public string IsSuccessSt => IsSuccess ? "موفق" : "ناموفق";

        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "تاریخ ایجاد")]
        public string CreateDateFa => CreateDate.ToPersianDateTime().ToString();



        public static Expression<Func<Domain.Entities.UserLog, UserActionLogDataTableDTO>> Selector
        {
            get
            {
                return model => new UserActionLogDataTableDTO()
                {
                    Id = model.Id,
                    UserIp = model.UserIp,
                    UserName = model.FullName,
                    ActionName = model.ActionName,
                    MenuName = model.MenuName,
                    Description = model.Description,
                    IsSuccess = model.IsSuccess,
                    CreateDate = model.CreateDate,
                    TargetId = model.TargetId
                };
            }
        }
    }
}
