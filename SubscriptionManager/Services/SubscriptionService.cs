using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Data;
using SubscriptionManager.Models;

namespace SubscriptionManager.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly AppDbContext _context;

        public SubscriptionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerSubscription>> GetAllCustomersAsync()
        {
            return await _context.CustomerSubscriptions
                .Include(c => c.SubscriptionType)
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<CustomerSubscription> GetCustomerByIdAsync(int id)
        {
            var customer = await _context.CustomerSubscriptions
                .Include(c => c.SubscriptionType)
                .FirstOrDefaultAsync(c => c.Id == id)
                .ConfigureAwait(false);

            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {id} not found.");

            return customer;
        }

        public async Task<CustomerSubscription> AddCustomerAsync(CustomerSubscription customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            customer.CreatedAt = DateTime.Now;

            // If no price per unit is set, use the default from settings
            if (customer.PricePerUnit <= 0)
            {
                var settings = await GetDefaultSettingsAsync().ConfigureAwait(false);
                customer.PricePerUnit = settings.DefaultPricePerUnit;
            }

            // Update monthly subscription fee if subscription type is set
            if (customer.SubscriptionTypeId.HasValue)
            {
                var subscriptionType = await _context.SubscriptionTypes
                    .FindAsync(customer.SubscriptionTypeId.Value)
                    .ConfigureAwait(false);

                if (subscriptionType != null)
                {
                    customer.MonthlySubscriptionFee = subscriptionType.MonthlyFee;
                    customer.CalculateTotalMonthlyBill();
                }
            }

            _context.CustomerSubscriptions.Add(customer);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return customer;
        }

        public async Task<CustomerSubscription> UpdateCustomerAsync(CustomerSubscription customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            customer.UpdatedAt = DateTime.Now;

            // Update monthly subscription fee based on subscription type
            if (customer.SubscriptionTypeId.HasValue)
            {
                var subscriptionType = await _context.SubscriptionTypes
                    .FindAsync(customer.SubscriptionTypeId.Value)
                    .ConfigureAwait(false);

                if (subscriptionType != null)
                {
                    customer.MonthlySubscriptionFee = subscriptionType.MonthlyFee;
                }
            }
            else
            {
                customer.MonthlySubscriptionFee = 0;
            }

            customer.CalculateTotalMonthlyBill();

            _context.CustomerSubscriptions.Update(customer);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return customer;
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.CustomerSubscriptions
                .FindAsync(id)
                .ConfigureAwait(false);

            if (customer != null)
            {
                customer.IsActive = false;
                customer.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<List<CounterHistory>> GetCustomerHistoryAsync(int customerId)
        {
            return await _context.CounterHistories
                .Where(h => h.CustomerSubscriptionId == customerId)
                .OrderByDescending(h => h.RecordDate)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task SaveReadingAsync(int customerId, decimal newReading, decimal pricePerUnit)
        {
            using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                var customer = await _context.CustomerSubscriptions
                    .FindAsync(customerId)
                    .ConfigureAwait(false);

                if (customer == null)
                    throw new InvalidOperationException($"Customer with ID {customerId} not found.");

                if (newReading <= customer.NewCounter)
                    throw new InvalidOperationException("New reading must be greater than current reading.");

                var consumption = newReading - customer.NewCounter;
                var billAmount = consumption * pricePerUnit;

                // Create history record with the price per unit used for this specific reading
                // This preserves the pricing at the time the reading was taken
                var history = new CounterHistory
                {
                    CustomerSubscriptionId = customerId,
                    OldCounter = customer.NewCounter,
                    NewCounter = newReading,
                    BillAmount = billAmount,
                    RecordDate = DateTime.Now,
                    PricePerUnit = pricePerUnit // This preserves the historical pricing
                };
                _context.CounterHistories.Add(history);

                // Update customer's current readings and billing info
                customer.OldCounter = customer.NewCounter;
                customer.NewCounter = newReading;
                customer.BillAmount = billAmount;
                customer.LastBillDate = DateTime.Now;
                customer.UpdatedAt = DateTime.Now;

                // Update the customer's price per unit to match what was used for this reading
                // This ensures consistency between the customer record and the reading
                customer.PricePerUnit = pricePerUnit;

                await _context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
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

        public async Task UpdateCustomerSubscriptionAsync(int customerId, int? subscriptionTypeId)
        {
            var customer = await _context.CustomerSubscriptions
                .FindAsync(customerId)
                .ConfigureAwait(false);

            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {customerId} not found.");

            customer.SubscriptionTypeId = subscriptionTypeId;

            if (subscriptionTypeId.HasValue)
            {
                var subscriptionType = await _context.SubscriptionTypes
                    .FindAsync(subscriptionTypeId.Value)
                    .ConfigureAwait(false);

                if (subscriptionType != null)
                {
                    customer.MonthlySubscriptionFee = subscriptionType.MonthlyFee;
                }
            }
            else
            {
                customer.MonthlySubscriptionFee = 0;
            }

            customer.CalculateTotalMonthlyBill();
            customer.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the default settings from the database.
        /// This is used to apply default pricing to new customers.
        /// </summary>
        /// <returns>The current settings</returns>
        private async Task<Settings> GetDefaultSettingsAsync()
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
    }
}