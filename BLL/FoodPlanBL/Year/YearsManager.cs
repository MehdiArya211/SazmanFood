using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace BLL;

public class YearsManager : Manager<Years, ApplicationContext>, IYearsManager
{
    public YearsManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
    {
    }
    public long? GetActiveYearId()
    {
        return UOW.Years
            .Get(x => x.IsActive)
            .Select(x => x.Id)
            .SingleOrDefault();
    }

}
