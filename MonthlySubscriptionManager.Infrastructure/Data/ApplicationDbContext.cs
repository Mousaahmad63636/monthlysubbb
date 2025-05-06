// MonthlySubscriptionManager.Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using MonthlySubscriptionManager.Domain.Entities;
using MonthlySubscriptionManager.Infrastructure.Data.Configurations;
using QuickTechSystems.Infrastructure.Data.Configurations;
using System.Reflection;

namespace MonthlySubscriptionManager.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerSubscription> CustomerSubscriptions => Set<CustomerSubscription>();
        public DbSet<SubscriptionPayment> SubscriptionPayments => Set<SubscriptionPayment>();
        public DbSet<CounterHistory> CounterHistories => Set<CounterHistory>();
        public DbSet<MonthlySubscriptionSettings> MonthlySubscriptionSettings { get; set; }
        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ApplyConfiguration(new MonthlySubscriptionSettingsConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerSubscriptionConfiguration());
        }
    }
}