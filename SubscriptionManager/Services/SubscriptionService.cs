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
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<CustomerSubscription> GetCustomerByIdAsync(int id)
        {
            return await _context.CustomerSubscriptions.FindAsync(id);
        }

        public async Task<CustomerSubscription> AddCustomerAsync(CustomerSubscription customer)
        {
            _context.CustomerSubscriptions.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<CustomerSubscription> UpdateCustomerAsync(CustomerSubscription customer)
        {
            customer.UpdatedAt = DateTime.Now;
            _context.CustomerSubscriptions.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.CustomerSubscriptions.FindAsync(id);
            if (customer != null)
            {
                customer.IsActive = false;
                customer.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CounterHistory>> GetCustomerHistoryAsync(int customerId)
        {
            return await _context.CounterHistories
                .Where(h => h.CustomerSubscriptionId == customerId)
                .OrderByDescending(h => h.RecordDate)
                .ToListAsync();
        }

        public async Task SaveReadingAsync(int customerId, decimal newReading, decimal pricePerUnit)
        {
            var customer = await _context.CustomerSubscriptions.FindAsync(customerId);
            if (customer == null) return;

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

            await _context.SaveChangesAsync();
        }
    }
}