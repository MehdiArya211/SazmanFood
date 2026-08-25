using Domain.Entities.Garrison;
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
    /// ایام هفته   
    /// </summary>
    //public enum Days
    //{
    //    [Description("شنبه")]
    //    Saturday = 1,
    //    [Description("یکشنبه")]
    //    Sunday = 2,
    //    [Description("دو شنبه ")]
    //    Monday = 3,
    //    [Description("سه‌ شنبه")]
    //    Tuesday = 4,
    //    [Description("چهار شنبه")]
    //    Wednesday = 5,

    //    [Description("پنج شنبه ")]
    //    Thursday = 6,
    //    [Description(" جمعه ")]
    //    Friday = 7

    //}

    public class Days
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

        public List<FoodPlanDay> FoodPlanDay { get; set; }
        public List<OrganGarrisonType> QoutaAllocation { get; set; }
        #endregion
    }

}
