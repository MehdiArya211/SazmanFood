using Domain.Entities;
using Domain.Entities.LogSystem;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class LogContext : DbContext
    {
        public LogContext(DbContextOptions<LogContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


        public DbSet<UserLog> UserLogs { get; set; }

        public DbSet<TrackDatabaseLog> UserChangeLogEvents { get; set; }
    }
}
