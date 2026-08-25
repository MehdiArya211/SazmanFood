using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Garrison
{
    /// <summary>
    /// یگان های پادگان
    /// </summary>
    public class OrganGarrison : EntityBase
    {

        public string Title { get; set; }
        public long? OrgId { get; set; }
        public long? ParentId { get; set; }
        public long OrganGarrisonTypeId { get; set; }

        public int? Code { get; set; }
        public int? SortName { get; set; }
        public bool IsDeleted { get; set; }
        public int EstedadKadr { get; set; }
        public int EstedadVazife { get; set; }

        #region Relation
        public OrganGarrison Parent { get; set; }
        public OrganGarrisonType OrganGarrisonType { get; set; }
        public List<QoutaPerson> QoutaPerson { get; set; }

        public List<Person> Person { get; set; }


        #endregion
    }
}
