using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class LogContextFactory : IDbContextFactory
    {
        public DbContext DbContext { get; }
        public LogContextFactory(LogContext context)
        {
            DbContext = context;
        }
    }
}
