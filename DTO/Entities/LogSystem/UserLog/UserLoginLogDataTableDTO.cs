using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Utilities.Extentions;

namespace DTO.UserLog
{
    public class UserLoginLogDataTableDTO
    {
        [Display(Name = "شناسه")]
        public long Id { get; set; }


        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }


        [Display(Name = "آیپی")]
        public string UserIp { get; set; }


        [Display(Name = "نام فعالیت")]
        public string ActionName { get; set; }


        [Display(Name = "توضیحات")]
        public string Description { get; set; }


        [Display(Name = "وضعیت ورود")]
        public bool IsSuccess { get; set; }
        public string IsSuccessSt => IsSuccess ? "موفق" : "ناموفق";



        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "تاریخ ایجاد")]
        public string CreateDateFa => CreateDate.ToPersianDateTime().ToString();



        public static Expression<Func<Domain.Entities.UserLog, UserLoginLogDataTableDTO>> Selector
        {
            get
            {
                return model => new UserLoginLogDataTableDTO()
                {
                    Id = model.Id,
                    UserName = model.FullName,
                    ActionName = model.ActionName,
                    Description = model.Description,
                    IsSuccess = model.IsSuccess,
                    CreateDate = model.CreateDate,
                    UserIp = model.UserIp
                };
            }
        }
    }
}
