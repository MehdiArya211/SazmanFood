using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FajrLog.DTO
{
    /// <summary>
    /// روی اینام نوع رویداد قرار میگیرد و جزییات نوع رویداد را مشخص میکند.
    /// </summary>
    public class FajrActionTypeInfoAttribute : Attribute
    {
        /// <summary>
        /// آیدی رویداد
        /// </summary>
        public long ActionId { get; set; }

        /// <summary>
        /// نوع رویداد
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// نوع زیر رویداد
        /// </summary>
        public string ActionSubType { get; set; }

        /// <summary>
        /// توضیحات اکشن
        /// </summary>
        public string Desc { get; set; }

    }
}
