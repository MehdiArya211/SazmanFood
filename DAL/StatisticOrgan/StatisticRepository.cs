using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
	public class StatisticRepository : Repository<Statistic>, IStatisticRepository
	{
		public StatisticRepository(DbContext _Context) : base(_Context)
		{
		}
	}
}
