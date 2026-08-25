using BLL.Interface;
using Domain.Entities;
using Infrastructure.Data;

namespace BLL
{
	public interface IStatisticManager : IManager<Statistic, ApplicationContext>
	{
	}
}
