using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extentions
{
    /// <summary>
    /// روی اینام قرار میگیرد و نوع داده را مشخص میکند.
    /// </summary>
    public class CustomDataTypeAttribute : Attribute
    {
        /// <summary>
        /// نوع داده عددی،متنی، بولین، دراپدون و ...
        /// </summary>
        public CustomDataType Type { get; set; }

        /// <summary>
        /// برای داده عددی حداقل مقدار
        /// <para>برای داده متنی حداقل طول رشته</para>
        /// </summary>
        public int Min { get; set; }


        /// <summary>
        /// برای داده عددی حداکثر مقدار
        /// <para>برای داده متنی حداکثر طول رشته</para>
        /// </summary>
        public int Max { get; set; }


        /// <summary>
        /// مقادیر برای دراپدون
        /// </summary>
        public string[] Dropdown { get; set; }


        /// <summary>
        /// توضیحات لازم برای placeholder
        /// </summary>
        public string Placeholder { get; set; }


        /// <summary>
        /// مقدار پیشفرض برای seed method
        /// </summary>
        public string DefultValue { get; set; }
    }
}
