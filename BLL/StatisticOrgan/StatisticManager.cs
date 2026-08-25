using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
	public class StatisticManager : Manager<Statistic, ApplicationContext>, IStatisticManager
	{
		protected readonly IHttpContextAccessor httpContextAccessor;
		protected readonly ISession Session;
		public StatisticManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor) : base(_Context, httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
			Session = httpContextAccessor.HttpContext.Session;
		}
	}
}
