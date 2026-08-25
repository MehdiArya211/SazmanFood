using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities
{
    /// <summary>
    /// فصل   
    /// </summary>
    //public enum Season
    //{
    //    [Description("بهار")]
    //    Spring = 1,
    //    [Description("تابستان")]
    //    Summer = 2,

    //    [Description("پاییز ")]
    //    Autumn = 3,
    //    [Description("زمستان ")]
    //    Winter = 4

    //}

    public class Season
    {
        [Key]
        [Display(Name = "شناسه")]
        public long Id { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(50, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Title { get; set; }

        [Display(Name = "کد")]
        public int Code { get; set; }

        public int SortName { get; set; }

        #region Relation

        public List<FoodPlan> FoodPlan { get; set; }

        #endregion
    }

}
