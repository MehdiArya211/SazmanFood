using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationContextFactory : IDbContextFactory
    {
        public DbContext DbContext { get; }
        public ApplicationContextFactory(ApplicationContext context)
        {
            DbContext = context;
        }
    }
}
