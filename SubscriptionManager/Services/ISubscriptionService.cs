using SubscriptionManager.Models;

namespace SubscriptionManager.Services
{
    public interface ISubscriptionService
    {
        Task<List<CustomerSubscription>> GetAllCustomersAsync();
        Task<CustomerSubscription> GetCustomerByIdAsync(int id);
        Task<CustomerSubscription> AddCustomerAsync(CustomerSubscription customer);
        Task<CustomerSubscription> UpdateCustomerAsync(CustomerSubscription customer);
        Task DeleteCustomerAsync(int id);
        Task<List<CounterHistory>> GetCustomerHistoryAsync(int customerId);
        Task SaveReadingAsync(int customerId, decimal newReading, decimal pricePerUnit);
        Task<List<SubscriptionType>> GetActiveSubscriptionTypesAsync();
        Task UpdateCustomerSubscriptionAsync(int customerId, int? subscriptionTypeId);
    }
}