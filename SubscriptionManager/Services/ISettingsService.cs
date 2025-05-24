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
    }
}