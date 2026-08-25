using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Redis
{
    /// <summary>
    /// ذخیره اطلاعات دفعات لاگین درون ردیس
    /// </summary>
    public class LoginLogDTO
    {
        /// <summary>
        /// تعداد دفعات تلاش
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// تاریخ آخرین ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }


        public LoginLogDTO(int count)
        {
            CreateDate = DateTime.Now;
            Count = count;
        }

        public LoginLogDTO()
        {
            CreateDate = DateTime.Now;
        }
    }

   
}
