using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DataTable
{
    public class DataTableSearchDTO
    {
        /// <summary>
        /// شماره شروع صفحه.
        /// صفحه اول از صفر شروع میشه و صفحه دوم از 10
        /// </summary>
        public int start { get; set; }

        /// <summary>
        /// تعداد آیتم های هر صفحه
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// متن سرچ شده
        /// </summary>
        public string searchValue { get; set; }

        /// <summary>
        /// اسم ستون سورت شده
        /// </summary>
        public string sortColumnName { get; set; }

        /// <summary>
        /// سورت صعودی هست یا نزولی
        /// </summary>
        public string sortDirection { get; set; }

        /// <summary>
        /// نحوه رسم دیتا تیبل برای پاس دادن در خروجی
        /// </summary>
        public string draw { get; set; }
    }
}
