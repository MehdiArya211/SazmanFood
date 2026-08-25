using Domain.Entities.Garrison;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class OrganGarrisionTypeManager : Manager<OrganGarrisonType, ApplicationContext>, IOrganGarrisionTypeManager
	{
		protected readonly IHttpContextAccessor httpContextAccessor;
		protected readonly ISession Session;
		public OrganGarrisionTypeManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor) : base(_Context, httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
			Session = httpContextAccessor.HttpContext.Session;
		}
	}
}
