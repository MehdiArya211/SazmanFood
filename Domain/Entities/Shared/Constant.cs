using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// پارامترهای ثابتی که بصورت کلید و مقدار تعریف می شوند
    /// </summary>
    public class Constant : EntityBase
    {

        [Display(Name = "عنوان")]
        [StringLength(100, ErrorMessage = "{0} حداکثر {1} کاراکتر باشد.")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }


        [Display(Name = "نوع پارامتر")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public ConstantType Type { get; set; }


        [Display(Name = "مقدار")]
        [StringLength(300, ErrorMessage = "{0} حداکثر {1} کاراکتر باشد.")]
        public string Value { get; set; }


        public Constant() : base()
        {

        }
    }
}
