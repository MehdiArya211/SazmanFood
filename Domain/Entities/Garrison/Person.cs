using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Garrison
{
    public class Person : EntityBase
    {
        public long OrganGarrisonId { get; set; }

        public long PersonTypeId { get; set; }

        public string FullName { get; set; }

        public int? PersonCode { get; set; }

        public int? NationalCode { get; set; }

        #region Relation

        public OrganGarrison OrganGarrison { get; set; }

        public PersonalType PersonalType { get; set; }
        #endregion
    }
}
