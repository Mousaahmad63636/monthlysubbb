using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Models;

namespace SubscriptionManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CustomerSubscription> CustomerSubscriptions { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<CounterHistory> CounterHistories { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CustomerSubscription configuration
            modelBuilder.Entity<CustomerSubscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.OldCounter).HasPrecision(18, 2);
                entity.Property(e => e.NewCounter).HasPrecision(18, 2);
                entity.Property(e => e.BillAmount).HasPrecision(18, 2);
                entity.Property(e => e.PricePerUnit).HasPrecision(18, 2);
                entity.Property(e => e.MonthlySubscriptionFee).HasPrecision(18, 2);
                entity.Property(e => e.TotalMonthlyBill).HasPrecision(18, 2);

                // Foreign key relationship
                entity.HasOne(e => e.SubscriptionType)
                      .WithMany(st => st.CustomerSubscriptions)
                      .HasForeignKey(e => e.SubscriptionTypeId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Add index for better performance
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.SubscriptionTypeId);
            });

            // Expense configuration
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);

                // Add index for better performance
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.Category);
            });

            // CounterHistory configuration
            modelBuilder.Entity<CounterHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OldCounter).HasPrecision(18, 2);
                entity.Property(e => e.NewCounter).HasPrecision(18, 2);
                entity.Property(e => e.BillAmount).HasPrecision(18, 2);
                entity.Property(e => e.PricePerUnit).HasPrecision(18, 2);

                entity.HasOne(e => e.CustomerSubscription)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerSubscriptionId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Add index for better performance
                entity.HasIndex(e => e.CustomerSubscriptionId);
                entity.HasIndex(e => e.RecordDate);
            });

            // Settings configuration
            modelBuilder.Entity<Settings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AdminEmail).HasMaxLength(100);
                entity.Property(e => e.DefaultPricePerUnit).HasPrecision(18, 2);
                entity.Property(e => e.BillingDay).HasDefaultValue(1);
            });

            // SubscriptionType configuration
            modelBuilder.Entity<SubscriptionType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.MonthlyFee).HasPrecision(18, 2);
                entity.Property(e => e.Category).HasMaxLength(50);

                // Add index for better performance
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Category);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SubscriptionManager;Trusted_Connection=True;MultipleActiveResultSets=true");
            }

            // Enable sensitive data logging in debug mode
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
#endif
        }
    }
}