using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Base
{
    /// <summary>
    /// مدل لازم برای نمایش خطا
    /// </summary>
    public class ErrorPageDTO
    {
        /// <summary>
        /// کد خطا مثل 404، 500 و ...
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// عنوان خطا
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// توضیحات خطا
        /// </summary>
        public string Description { get; set; }


        #region سازنده
        public ErrorPageDTO(string errorCode, string title, string description)
        {
            ErrorCode = errorCode;
            Title = title;
            Description = description;
        }

        public ErrorPageDTO()
        {

        } 
        #endregion
    }
}
