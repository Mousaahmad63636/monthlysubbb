using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuickTechSystems.Domain.Entities;

namespace QuickTechSystems.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
   
        IGenericRepository<CustomerSubscription> CustomerSubscriptions { get; }
        IGenericRepository<SubscriptionType> SubscriptionTypes { get; }
        IGenericRepository<MonthlySubscriptionSettings> MonthlySubscriptionSettings { get; }

        Task<int> SaveChangesAsync();
    }
}