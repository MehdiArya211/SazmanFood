using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.FoodManage
{
    /// <summary>
    /// نوع ژتون غذا
    /// </summary>
    public class FoodTokenType
    {
        [Key]
        [Display(Name = "شناسه")]
        public long Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(50, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "کد")]
        public int Code { get; set; }

        public int SortName { get; set; }

        public bool IsDeleted { get; set; }

        #region Relation

        public List<QoutaPerson> QoutaPersons { get; set; }

        #endregion
    }
}