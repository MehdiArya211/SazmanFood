using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DataTable
{
    /// <summary>
    /// خروجی ارسالی برای ایجاد دیتاتیبل
    /// </summary>
    public class DataTableResponseDTO<TEntity>
    {
        public string draw { get; set; }

        /// <summary>
        /// تعداد کل آیتم ها
        /// </summary>
        public int recordsTotal { get; set; }

        /// <summary>
        /// تعداد آیتم ها بعد از جستجو
        /// </summary>
        public int recordsFiltered { get; set; }

        /// <summary>
        /// داده خروجی برای ایجاد دیتاتیبل
        /// </summary>
        public IEnumerable<TEntity> data { get; set; }


        public dynamic AdditionalData { get; set; }
    }
}
