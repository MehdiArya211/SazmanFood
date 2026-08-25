using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public interface IDbContextFactory
    {
        DbContext DbContext { get; }
    }
}
