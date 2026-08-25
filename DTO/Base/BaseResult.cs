using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Base
{
    public class BaseResult
    {
        /// <summary>
        /// وضعیت عملیات
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// پیغام مناسب بخصوص در هنگام خطا
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// آبجکتی که در خروجی برمیگردد
        /// </summary>
        public dynamic Model { get; set; }


        public BaseResult()
        {

        }

        public BaseResult(bool _status , string _message , object _model = null)
        {
            Status = _status;
            Message = _message;
            Model = _model;
        }
    }
}
