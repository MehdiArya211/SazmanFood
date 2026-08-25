using FajrLog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region DeleteBehavior - رفتار در هنگام حذف دیتا
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            #endregion


        }


        #region جداول دیتابیسی

        /// <summary>
        /// لاگ فجر
        /// </summary>
        public DbSet<FajrLogEntity> FajrLog { get; set; }
        #endregion





    }
}
