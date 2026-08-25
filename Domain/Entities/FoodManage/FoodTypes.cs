using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities.FoodManage
{
    /// <summary>
    /// نوع غذا
    /// </summary>
    public class FoodTypes
    {
        [Key]
        [Display(Name = "شناسه")]
        public long Id { get; set; }
        public string Title { get; set; }
        public int? Code { get; set; }
        public int? SortName { get; set; }
        public bool IsDeleted { get; set; }

        #region Relation
        public ICollection<Foods> Foods { get; set; }

        #endregion
    }
}
