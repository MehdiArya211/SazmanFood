using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Garrison
{
    /// <summary>
    /// نوع یگان های پادگان
    /// </summary>
    public class OrganGarrisonType
    {
        [Key]
        [Display(Name = "شناسه")]
        public long Id { get; set; }
        public string Title { get; set; }
        public int? Code { get; set; }
        public int? SortName { get; set; }
        public bool IsDeleted { get; set; }
        #region Relation
        public List<OrganGarrison> OrganGarrisons { get; set; }
        #endregion
    }
}
