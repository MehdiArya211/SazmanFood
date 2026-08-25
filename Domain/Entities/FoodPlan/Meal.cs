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
    /// وعده غذایی  
    /// </summary>
    //public enum Meal
    //{
    //    [Description("صبحانه")]
    //    Breakfast = 1,
    //    [Description("ناهار")]
    //    Lunch = 2,

    //    [Description("شام ")]
    //    Dinner = 3
    //}

    public class Meal
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
        public List<OrganGarrisonType> QoutaAllocations { get; set; }

        #endregion
    }

}
