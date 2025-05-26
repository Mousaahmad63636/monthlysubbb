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
                // Create default settings
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

            using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                // Get the current settings to compare price changes
                var currentSettings = await _context.Settings.FirstOrDefaultAsync().ConfigureAwait(false);

                if (currentSettings != null && currentSettings.DefaultPricePerUnit != settings.DefaultPricePerUnit)
                {
                    // Update all active customers' price per unit to match the new default
                    await UpdateAllCustomersPricingAsync(settings.DefaultPricePerUnit).ConfigureAwait(false);
                }

                settings.UpdatedAt = DateTime.Now;
                _context.Settings.Update(settings);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                await transaction.CommitAsync().ConfigureAwait(false);
                return settings;
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Updates the price per unit for all active customers to match the new default pricing.
        /// This ensures future readings use the updated pricing while preserving historical rates.
        /// </summary>
        /// <param name="newPricePerUnit">The new default price per unit to apply to all customers</param>
        private async Task UpdateAllCustomersPricingAsync(decimal newPricePerUnit)
        {
            var activeCustomers = await _context.CustomerSubscriptions
                .Where(cs => cs.IsActive)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var customer in activeCustomers)
            {
                customer.PricePerUnit = newPricePerUnit;
                customer.UpdatedAt = DateTime.Now;
            }

            // Save changes for all customers in a single operation
            await _context.SaveChangesAsync().ConfigureAwait(false);
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

            // Check for duplicate names (excluding the current record)
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
                    // If customers are using this type, deactivate it instead of deleting
                    subscriptionType.IsActive = false;
                    subscriptionType.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // If no customers are using it, safe to delete
                    _context.SubscriptionTypes.Remove(subscriptionType);
                }

                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the count of customers that will be affected by a price change.
        /// This is useful for displaying confirmation messages to users.
        /// </summary>
        /// <returns>Number of active customers</returns>
        public async Task<int> GetActiveCustomersCountAsync()
        {
            return await _context.CustomerSubscriptions
                .CountAsync(cs => cs.IsActive)
                .ConfigureAwait(false);
        }
    }
}