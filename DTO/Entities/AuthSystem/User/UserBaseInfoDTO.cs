using DTO.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text;

namespace DTO.User
{
    public class UserBaseInfoDTO : EntityDTO
    {

        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Display(Name = "نام")]
        public string Name { get; set; }

        

        /// <summary>
        /// فیلدهای لازم برای استخراج مدل از موجودیت مربوطه
        /// </summary>
        public static Expression<Func<Domain.Entities.User, UserBaseInfoDTO>> Selector
        {
            get
            {
                return model => new UserBaseInfoDTO()
                {
                    Id = model.Id,
                    Username = model.Username,
                    Name = model.Name
                };
            }
        }


    }
}
