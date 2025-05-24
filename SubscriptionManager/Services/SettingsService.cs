using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Data;
using SubscriptionManager.Models;

namespace SubscriptionManager.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _context;

        public SettingsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Settings> GetSettingsAsync()
        {
            var settings = await _context.Settings
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (settings == null)
            {
                // Create default settings if none exist
                settings = new Settings
                {
                    DefaultPricePerUnit = 1.0m,
                    CompanyName = "Subscription Manager",
                    AutoCalculateMonthlyFees = true,
                    BillingDay = 1,
                    AdminEmail = "",
                    CreatedAt = DateTime.Now
                };

                _context.Settings.Add(settings);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }

            return settings;
        }

        public async Task<Settings> UpdateSettingsAsync(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            settings.UpdatedAt = DateTime.Now;
            _context.Settings.Update(settings);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return settings;
        }

        public async Task<List<SubscriptionType>> GetAllSubscriptionTypesAsync()
        {
            return await _context.SubscriptionTypes
                .OrderBy(st => st.Name)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<List<SubscriptionType>> GetActiveSubscriptionTypesAsync()
        {
            return await _context.SubscriptionTypes
                .Where(st => st.IsActive)
                .OrderBy(st => st.Name)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<SubscriptionType> GetSubscriptionTypeByIdAsync(int id)
        {
            var subscriptionType = await _context.SubscriptionTypes
                .FindAsync(id)
                .ConfigureAwait(false);

            if (subscriptionType == null)
                throw new InvalidOperationException($"Subscription type with ID {id} not found.");

            return subscriptionType;
        }

        public async Task<SubscriptionType> AddSubscriptionTypeAsync(SubscriptionType subscriptionType)
        {
            if (subscriptionType == null)
                throw new ArgumentNullException(nameof(subscriptionType));

            // Check for duplicate names
            var existing = await _context.SubscriptionTypes
                .Where(st => st.Name.ToLower() == subscriptionType.Name.ToLower())
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (existing != null)
                throw new InvalidOperationException($"A subscription type with the name '{subscriptionType.Name}' already exists.");

            subscriptionType.CreatedAt = DateTime.Now;
            _context.SubscriptionTypes.Add(subscriptionType);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return subscriptionType;
        }

        public async Task<SubscriptionType> UpdateSubscriptionTypeAsync(SubscriptionType subscriptionType)
        {
            if (subscriptionType == null)
                throw new ArgumentNullException(nameof(subscriptionType));

            // Check for duplicate names (excluding current record)
            var existing = await _context.SubscriptionTypes
                .Where(st => st.Name.ToLower() == subscriptionType.Name.ToLower() && st.Id != subscriptionType.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (existing != null)
                throw new InvalidOperationException($"A subscription type with the name '{subscriptionType.Name}' already exists.");

            subscriptionType.UpdatedAt = DateTime.Now;
            _context.SubscriptionTypes.Update(subscriptionType);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return subscriptionType;
        }

        public async Task DeleteSubscriptionTypeAsync(int id)
        {
            var subscriptionType = await _context.SubscriptionTypes
                .FindAsync(id)
                .ConfigureAwait(false);

            if (subscriptionType != null)
            {
                // Check if any customers are using this subscription type
                var customersUsingType = await _context.CustomerSubscriptions
                    .Where(cs => cs.SubscriptionTypeId == id && cs.IsActive)
                    .CountAsync()
                    .ConfigureAwait(false);

                if (customersUsingType > 0)
                {
                    // Soft delete - just mark as inactive
                    subscriptionType.IsActive = false;
                    subscriptionType.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Hard delete if no customers are using it
                    _context.SubscriptionTypes.Remove(subscriptionType);
                }

                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}