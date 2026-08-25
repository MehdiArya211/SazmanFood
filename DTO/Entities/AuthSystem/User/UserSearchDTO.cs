using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DTO.User
{
    /// <summary>
    /// مدل جستجو برای کاربران در selectize
    /// </summary>
    public class UserSearchDTO
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Mobile { get; set; }
        
        public string Role { get; set; }

        public UserType Type { get; set; }



        /// <summary>
        /// فیلدهای لازم برای استخراج مدل از موجودیت مربوطه
        /// </summary>
        public static Expression<Func<Domain.Entities.User, UserSearchDTO>> Selector
        {
            get
            {
                return model => new UserSearchDTO()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Username = model.Username,
                    Mobile = model.Mobile,
                    Role = model.Role.Title,
                    Type = model.Type,
                };
            }
        }



    }
}
