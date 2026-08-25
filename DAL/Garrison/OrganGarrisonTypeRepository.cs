using Domain.Entities.Garrison;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class OrganGarrisonTypeRepository : Repository<OrganGarrisonType>, IOrganGarrisonTypeRepository
	{
		public OrganGarrisonTypeRepository(DbContext _Context) : base(_Context)
		{
		}
	}
}
