using SubscriptionManager.Models;

namespace SubscriptionManager.Services
{
    public interface ISettingsService
    {
        Task<Settings> GetSettingsAsync();
        Task<Settings> UpdateSettingsAsync(Settings settings);
        Task<List<SubscriptionType>> GetAllSubscriptionTypesAsync();
        Task<SubscriptionType> GetSubscriptionTypeByIdAsync(int id);
        Task<SubscriptionType> AddSubscriptionTypeAsync(SubscriptionType subscriptionType);
        Task<SubscriptionType> UpdateSubscriptionTypeAsync(SubscriptionType subscriptionType);
        Task DeleteSubscriptionTypeAsync(int id);
        Task<List<SubscriptionType>> GetActiveSubscriptionTypesAsync();

        /// <summary>
        /// Gets the count of active customers that will be affected by pricing changes.
        /// </summary>
        /// <returns>Number of active customers</returns>
        Task<int> GetActiveCustomersCountAsync();
    }
}