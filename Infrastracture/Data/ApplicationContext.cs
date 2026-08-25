using Domain.Entities;
using Domain.Entities.FoodManage;
using Domain.Entities.FoodReservation;
using Domain.Entities.Garrison;
using Domain.Entities.LogSystem;
using Domain.Enums;
using Infrastracture.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
            base.OnModelCreating(modelBuilder);

            #region Fluent API - لود تنظیمات جانبی

            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly());

            #endregion

            #region رفتار پیش‌فرض حذف

            /*
             * ابتدا تمام روابط Restrict می‌شوند.
             * روابطی که واقعاً باید Cascade باشند بعداً بازنویسی می‌شوند.
             */
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(x => x.GetForeignKeys()))
            {
                relationship.DeleteBehavior =
                    DeleteBehavior.Restrict;
            }

            #endregion

            #region آمار و سهمیه یگان

            modelBuilder.Entity<UnitStatistic>()
                .ToTable("UnitStatistics");

            modelBuilder.Entity<UnitStatisticDetail>()
                .ToTable("UnitStatisticDetails");

            modelBuilder.Entity<UnitQuota>()
                .ToTable("UnitQuotas");

            #region رابطه آمار و جزئیات

            modelBuilder.Entity<UnitStatisticDetail>()
                .HasOne(x => x.UnitStatistic)
                .WithMany(x => x.Details)
                .HasForeignKey(x => x.UnitStatisticId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region رابطه جزئیات و نوع پرسنل

            modelBuilder.Entity<UnitStatisticDetail>()
                .HasOne(x => x.PersonalType)
                .WithMany(x => x.UnitStatisticDetails)
                .HasForeignKey(x => x.PersonalTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region رابطه جزئیات و نوع خدمت

            modelBuilder.Entity<UnitStatisticDetail>()
                .HasOne(x => x.YeganType)
                .WithMany(x => x.UnitStatisticDetails)
                .HasForeignKey(x => x.YeganTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region رابطه سهمیه و آمار

            modelBuilder.Entity<UnitQuota>()
                .HasOne(x => x.UnitStatistic)
                .WithMany(x => x.Quotas)
                .HasForeignKey(x => x.UnitStatisticId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region رابطه سهمیه و نوع پرسنل

            modelBuilder.Entity<UnitQuota>()
                .HasOne(x => x.PersonalType)
                .WithMany(x => x.UnitQuotas)
                .HasForeignKey(x => x.PersonalTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region رابطه سهمیه و نوع خدمت

            modelBuilder.Entity<UnitQuota>()
                .HasOne(x => x.YeganType)
                .WithMany(x => x.UnitQuotas)
                .HasForeignKey(x => x.YeganTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region رابطه سهمیه و مأخذ غذایی

            modelBuilder.Entity<UnitQuota>()
                .HasOne(x => x.FoodSource)
                .WithMany(x => x.UnitQuotas)
                .HasForeignKey(x => x.FoodSourceId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region مقدار پیش‌فرض نوع روز

            modelBuilder.Entity<FoodSource>()
                .Property(x => x.DayType)
                .HasDefaultValue(DayType.Normal);

            #endregion

            #region ایندکس موقت مأخذ غذایی

            /*
             * فعلاً Unique نیست، چون داده‌های قدیمی باید تعیین نوع روز شوند.
             * پس از پاک‌سازی داده‌ها در Migration بعدی IsUnique اضافه می‌شود.
             */
            modelBuilder.Entity<FoodSource>()
                .HasIndex(x => new
                {
                    x.YeganTypeId,
                    x.PersonalTypeId,
                    x.DayType
                });

            #endregion

            #region ایندکس یکتای جزئیات

            modelBuilder.Entity<UnitStatisticDetail>()
                .HasIndex(x => new
                {
                    x.UnitStatisticId,
                    x.PersonalTypeId,
                    x.YeganTypeId
                })
                .IsUnique();

            #endregion

            #region ایندکس یکتای سهمیه

            modelBuilder.Entity<UnitQuota>()
                .HasIndex(x => new
                {
                    x.UnitStatisticId,
                    x.PersonalTypeId,
                    x.YeganTypeId,
                    x.DayType
                })
                .IsUnique();

            #endregion

            #region فقط یک آمار تأییدشده فعال

            modelBuilder.Entity<UnitStatistic>()
                .HasIndex(x => x.OrgId)
                .IsUnique()
                .HasFilter(
                    "[IsActive] = 1 AND [Status] = 3");

            #endregion

            #endregion

            #region UnitQuotaPerson

            modelBuilder.Entity<UnitQuotaPerson>()
                .ToTable("UnitQuotaPersons");

            modelBuilder.Entity<UnitQuotaPerson>()
                .HasOne(x => x.UnitStatistic)
                .WithMany()
                .HasForeignKey(x => x.UnitStatisticId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnitQuotaPerson>()
                .HasOne(x => x.UnitQuota)
                .WithMany(x => x.Persons)
                .HasForeignKey(x => x.UnitQuotaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnitQuotaPerson>()
                .HasOne(x => x.Meal)
                .WithMany()
                .HasForeignKey(x => x.MealId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnitQuotaPerson>()
                .HasOne(x => x.PersonalType)
                .WithMany()
                .HasForeignKey(x => x.PersonalTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnitQuotaPerson>()
                .HasOne(x => x.DiningHall)
                .WithMany()
                .HasForeignKey(x => x.DiningHallId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * جلوگیری از ثبت تکراری همان شخص،
             * همان وعده و همان سهمیه.
             *
             * UnitQuotaId نوع خدمت، نوع روز و نوع پرسنل
             * سهمیه را مشخص می‌کند.
             */
            modelBuilder.Entity<UnitQuotaPerson>()
                .HasIndex(x => new
                {
                    x.UnitQuotaId,
                    x.MealId,
                    x.PersonId
                })
                .IsUnique()
                .HasFilter("[PersonId] IS NOT NULL");

            modelBuilder.Entity<UnitQuotaPerson>()
                .HasIndex(x => new
                {
                    x.UnitQuotaId,
                    x.MealId,
                    x.PersonCode
                })
                .IsUnique()
                .HasFilter("[PersonCode] IS NOT NULL");

            modelBuilder.Entity<UnitQuotaPerson>()
                .HasIndex(x => new
                {
                    x.UnitQuotaId,
                    x.MealId,
                    x.NationalCode
                })
                .IsUnique()
                .HasFilter("[NationalCode] IS NOT NULL");

            #endregion

            #region UnitCalendar

            modelBuilder.Entity<UnitCalendar>()
                .ToTable("UnitCalendars");

            modelBuilder.Entity<UnitCalendar>()
                .Property(x => x.CalendarDate)
                .HasColumnType("date");

            modelBuilder.Entity<UnitCalendar>()
                .Property(x => x.DayType)
                .HasConversion<int>()
                .IsRequired();

            modelBuilder.Entity<UnitCalendar>()
                .Property(x => x.OrgTitle)
                .HasMaxLength(200);

            modelBuilder.Entity<UnitCalendar>()
                .Property(x => x.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<UnitCalendar>()
                .HasIndex(x => new
                {
                    x.OrgId,
                    x.CalendarDate
                })
                .IsUnique()
                            .HasFilter("[IsDeleted] = 0");

            #endregion

            #region GuestExtraFoodRequest

            modelBuilder.Entity<GuestExtraFoodRequest>()
                .ToTable("GuestExtraFoodRequests");

            modelBuilder.Entity<GuestExtraFoodRequest>()
                .Property(x => x.FromDate)
                .HasColumnType("date");

            modelBuilder.Entity<GuestExtraFoodRequest>()
                .Property(x => x.ToDate)
                .HasColumnType("date");

            modelBuilder.Entity<GuestExtraFoodRequest>()
                .HasOne(x => x.Meal)
                .WithMany()
                .HasForeignKey(x => x.MealId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GuestExtraFoodRequest>()
                .HasOne(x => x.PersonalType)
                .WithMany()
                .HasForeignKey(x => x.PersonalTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GuestExtraFoodRequest>()
                .HasOne(x => x.YeganType)
                .WithMany()
                .HasForeignKey(x => x.YeganTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GuestExtraFoodRequest>()
                .HasIndex(x => new
                {
                    x.OrgId,
                    x.MealId,
                    x.PersonalTypeId,
                    x.YeganTypeId,
                    x.Status,
                    x.FromDate,
                    x.ToDate
                });

            #endregion

            #region GuestExtraFoodRequestAttachment

            modelBuilder.Entity<GuestExtraFoodRequestAttachment>()
                .ToTable("GuestExtraFoodRequestAttachments");

            modelBuilder.Entity<GuestExtraFoodRequestAttachment>()
                .HasOne(x =>
                    x.GuestExtraFoodRequest)
                .WithMany(x =>
                    x.Attachments)
                .HasForeignKey(x =>
                    x.GuestExtraFoodRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GuestExtraFoodRequestAttachment>()
                .Property(x => x.FileContent)
                .HasColumnType("varbinary(max)");

            #endregion

            #region Extera Food person
            modelBuilder.Entity<GuestExtraFoodRequestPerson>()
    .HasIndex(x => new
    {
        x.GuestExtraFoodRequestId,
        x.PersonCode
    })
    .IsUnique()
    .HasFilter("[IsDeleted] = 0 AND [PersonCode] IS NOT NULL");

            modelBuilder.Entity<GuestExtraFoodRequestPerson>()
                .HasIndex(x => new
                {
                    x.GuestExtraFoodRequestId,
                    x.NationalCode
                })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0 AND [NationalCode] IS NOT NULL");
            #endregion

            #region Seed Data

            SeedData.InitialSeedData(
                ref modelBuilder);

            #endregion
        }


        #region جداول دیتابیسی

        #region AuthSystem

        /// <summary>
        /// منو ها
        /// </summary>
        public DbSet<Menu> Menus { get; set; }

        /// <summary>
        /// نقش ها
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// دسترسی منو ها
        /// </summary>
        public DbSet<RoleMenu> RoleMenus { get; set; }

        /// <summary>
        /// پرسنل
        /// </summary>
        public DbSet<User> Users { get; set; }

        public DbSet<UserPasswordHistory> UserPasswordHistorys { get; set; }
      
        #endregion


        #region Shared
        /// <summary>
        /// مقادیر ثابت
        /// </summary>
        public DbSet<Constant> Constants { get; set; }
        #endregion


        #region SeriLog
        public DbSet<SeriLog> SeriLog { get; set; }

        #endregion

        #region غذا
        public DbSet<DiningHall> DiningHall { get; set; }
        public DbSet<Foods> Foods { get; set; }
        public DbSet<FoodTypes> FoodTypes { get; set; }
        public DbSet<Kitchens> Kitchens { get; set; }

        //************FoodPlan***************
        public DbSet<Days> Days { get; set; }
        public DbSet<FoodPlan> FoodPlan { get; set; }
        public DbSet<FoodPlanDay> FoodPlanDay { get; set; }
        public DbSet<FoodSource> FoodSource { get; set; }
        public DbSet<Meal> Meal { get; set; }
        public DbSet<PersonalType> PersonalType { get; set; }
        public DbSet<Season> Season { get; set; }
        public DbSet<Years> Years { get; set; }
        public DbSet<YeganType> YeganType { get; set; }
		//*******************************//


		#endregion

		#region  سهمیه بندی
		public DbSet<QoutaAllocation> QoutaAllocations { get; set; }
		public DbSet<QoutaPerson> QoutaPersons { get; set; }

		#endregion

		#region  یگان های پادگان
		public DbSet<OrganGarrison> OrganGarrison { get; set; }
		public DbSet<OrganGarrisonType> OrganGarrisonType { get; set; }

		#endregion

		#region آمار یگان های پادگان
		public DbSet<Statistic> Statistic { get; set; }

        #endregion

        #region FoodReserve
        public DbSet<FoodReserve> FoodReserves { get; set; }
        public DbSet<FoodReserveDetail> FoodReserveDetails { get; set; }

        #endregion

        #region Unit statistic - Unit Quota
        public DbSet<UnitStatistic> UnitStatistics { get; set; }

        public DbSet<UnitStatisticDetail> UnitStatisticDetails { get; set; }

        public DbSet<UnitQuota> UnitQuotas { get; set; }    
        #endregion

        #region ZP
        public DbSet<Devices> Devices { get; set; }
        public DbSet<Printer> Printer { get; set; }

        #endregion

        public DbSet<UnitCalendar> UnitCalendars { get; set; }

        public DbSet<GuestExtraFoodRequest>
    GuestExtraFoodRequests
        { get; set; }

        public DbSet<GuestExtraFoodRequestAttachment>
            GuestExtraFoodRequestAttachments
        { get; set; }

        #endregion

        #region Extera Food Perosn
        public DbSet<GuestExtraFoodRequestPerson>
    GuestExtraFoodRequestPersons
        { get; set; }
        #endregion




    }
}
