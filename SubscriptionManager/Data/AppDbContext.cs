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
            });

            // Expense configuration
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);
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
                      .HasForeignKey(e => e.CustomerSubscriptionId);
            });
        }
    }
}