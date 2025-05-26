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
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

                entity.HasOne(e => e.SubscriptionType)
                      .WithMany(st => st.CustomerSubscriptions)
                      .HasForeignKey(e => e.SubscriptionTypeId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.SubscriptionTypeId);
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.Category);
            });

            modelBuilder.Entity<CounterHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OldCounter).HasPrecision(18, 2);
                entity.Property(e => e.NewCounter).HasPrecision(18, 2);
                entity.Property(e => e.BillAmount).HasPrecision(18, 2);
                entity.Property(e => e.PricePerUnit).HasPrecision(18, 2);

                entity.HasOne(e => e.CustomerSubscription)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerSubscriptionId) // Fixed: was CustomerSubscrationId
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.CustomerSubscriptionId); // Fixed: was CustomerSubscrationId
                entity.HasIndex(e => e.RecordDate);
            });

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AdminEmail).HasMaxLength(100);
                entity.Property(e => e.DefaultPricePerUnit).HasPrecision(18, 2);
                entity.Property(e => e.BillingDay).HasDefaultValue(1);
            });

            modelBuilder.Entity<SubscriptionType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.MonthlyFee).HasPrecision(18, 2);
                entity.Property(e => e.Category).HasMaxLength(50);

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Category);
            });

            // Payment entity configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UsageBillAmount).HasPrecision(18, 2);
                entity.Property(e => e.MonthlySubscriptionAmount).HasPrecision(18, 2);
                entity.Property(e => e.TotalBillAmount).HasPrecision(18, 2);
                entity.Property(e => e.AmountPaid).HasPrecision(18, 2);
                entity.Property(e => e.RemainingBalance).HasPrecision(18, 2);
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.ReceiptNumber).HasMaxLength(100);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.CustomerSubscription)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerSubscriptionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.CustomerSubscriptionId);
                entity.HasIndex(e => e.PaymentDate);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsFullyPaid);
                entity.HasIndex(e => e.ReceiptNumber);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SubscriptionManager;Trusted_Connection=True;MultipleActiveResultSets=true");
            }

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
#endif
        }
    }
}