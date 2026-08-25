using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebApi
{
    public partial class CenterOrganizationAja
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int OrgId { get; set; }
        public string OrgTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public int? ProvinceId { get; set; }
        public int? CityId { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int? GeoUnitId { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string Description { get; set; }
        public string OrganizationType { get; set; }
        public DateTime CreateDate { get; set; }
        public int ServiceStatCd { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastEditDate { get; set; }
        public long? LastEditUserId { get; set; }
        public DateTime RegDate { get; set; }
        public long RegUserId { get; set; }
        public long? OrganizationId { get; set; }
        public Guid Guid { get; set; }
        public string ActionType { get; set; }
        public int? OrganizationCode { get; set; }
        public int? NiroCd { get; set; }
        public int? GeoUnitIdCity { get; set; }
    }
}
