using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    public class DbContexts
    {
        private Dictionary<string, IDbContextFactory> _dbContextFactories = new Dictionary<string, IDbContextFactory>();

        public DbContexts(IServiceProvider serviceProvider)
        {
            foreach (var type in typeof(IDbContextFactory).Assembly.GetTypes())
            {
                if (typeof(IDbContextFactory).IsAssignableFrom(type) && !type.IsInterface)
                {
                    _dbContextFactories.Add(type.Name.Replace("Factory", string.Empty),
                        (IDbContextFactory)ActivatorUtilities.CreateInstance(serviceProvider, type));
                }
            }
        }

        public DbContext this[string index] => _dbContextFactories[index].DbContext;
    }
}
