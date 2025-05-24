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
                .AsNoTracking() // Improve performance for read-only queries
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

            // If subscription type is assigned, set the monthly fee
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

            // If subscription type changed, update the monthly fee
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
                .AsNoTracking() // Improve performance for read-only queries
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

                // Validate new reading
                if (newReading <= customer.NewCounter)
                    throw new InvalidOperationException("New reading must be greater than current reading.");

                // Calculate consumption and bill
                var consumption = newReading - customer.NewCounter;
                var billAmount = consumption * pricePerUnit;

                // Save history
                var history = new CounterHistory
                {
                    CustomerSubscriptionId = customerId,
                    OldCounter = customer.NewCounter,
                    NewCounter = newReading,
                    BillAmount = billAmount,
                    RecordDate = DateTime.Now,
                    PricePerUnit = pricePerUnit
                };
                _context.CounterHistories.Add(history);

                // Update customer
                customer.OldCounter = customer.NewCounter;
                customer.NewCounter = newReading;
                customer.BillAmount = billAmount;
                customer.LastBillDate = DateTime.Now;
                customer.UpdatedAt = DateTime.Now;

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
    }
}