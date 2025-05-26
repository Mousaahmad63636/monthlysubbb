using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Data;
using SubscriptionManager.Models;

namespace SubscriptionManager.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetCustomerPaymentsAsync(int customerId)
        {
            return await _context.Payments
                .Where(p => p.CustomerSubscriptionId == customerId)
                .OrderByDescending(p => p.PaymentDate)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.CustomerSubscription)
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (payment == null)
                throw new InvalidOperationException($"Payment with ID {id} not found.");

            return payment;
        }

        public async Task<Payment> SettleCurrentBillAsync(int customerId, decimal amountPaid, string paymentMethod, string notes = "")
        {
            using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                var customer = await _context.CustomerSubscriptions
                    .FindAsync(customerId)
                    .ConfigureAwait(false);

                if (customer == null)
                    throw new InvalidOperationException($"Customer with ID {customerId} not found.");

                if (amountPaid <= 0)
                    throw new ArgumentException("Amount paid must be greater than zero.", nameof(amountPaid));

                // Create payment record
                var payment = new Payment
                {
                    CustomerSubscriptionId = customerId,
                    UsageBillAmount = customer.BillAmount,
                    MonthlySubscriptionAmount = customer.MonthlySubscriptionFee,
                    AmountPaid = amountPaid,
                    PaymentDate = DateTime.Now,
                    BillDate = customer.LastBillDate,
                    PaymentMethod = paymentMethod,
                    Notes = notes,
                    ReceiptNumber = await GenerateReceiptNumberAsync().ConfigureAwait(false),
                    CreatedAt = DateTime.Now
                };

                // Calculate total bill and remaining balance
                payment.CalculateTotalBill();

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                await transaction.CommitAsync().ConfigureAwait(false);
                return payment;
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task<Payment> AddPaymentAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            payment.CreatedAt = DateTime.Now;
            payment.ReceiptNumber = await GenerateReceiptNumberAsync().ConfigureAwait(false);
            payment.CalculateTotalBill();

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return payment;
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            payment.UpdatedAt = DateTime.Now;
            payment.CalculateTotalBill();

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return payment;
        }

        public async Task DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments
                .FindAsync(id)
                .ConfigureAwait(false);

            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<Payment?> GetCurrentBillAsync(int customerId)
        {
            var customer = await _context.CustomerSubscriptions
                .FindAsync(customerId)
                .ConfigureAwait(false);

            if (customer == null)
                return null;

            // Create a payment object representing the current bill
            var currentBill = new Payment
            {
                CustomerSubscriptionId = customerId,
                UsageBillAmount = customer.BillAmount,
                MonthlySubscriptionAmount = customer.MonthlySubscriptionFee,
                BillDate = customer.LastBillDate,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Cash", // Default
                Status = PaymentStatus.Pending
            };

            currentBill.CalculateTotalBill();
            return currentBill;
        }

        public async Task<List<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.CustomerSubscription)
                .Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.PartiallyPaid)
                .OrderByDescending(p => p.BillDate)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<PaymentStatistics> GetCustomerPaymentStatisticsAsync(int customerId)
        {
            var payments = await _context.Payments
                .Where(p => p.CustomerSubscriptionId == customerId)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            if (!payments.Any())
            {
                return new PaymentStatistics
                {
                    TotalPaid = 0,
                    TotalOutstanding = 0,
                    TotalPayments = 0,
                    PendingPayments = 0,
                    LastPaymentDate = null,
                    AveragePaymentAmount = 0
                };
            }

            return new PaymentStatistics
            {
                TotalPaid = payments.Sum(p => p.AmountPaid),
                TotalOutstanding = payments.Where(p => !p.IsFullyPaid).Sum(p => p.RemainingBalance),
                TotalPayments = payments.Count,
                PendingPayments = payments.Count(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.PartiallyPaid),
                LastPaymentDate = payments.Max(p => p.PaymentDate),
                AveragePaymentAmount = payments.Where(p => p.AmountPaid > 0).Average(p => p.AmountPaid)
            };
        }

        public async Task<decimal> GetTotalOutstandingBalanceAsync()
        {
            return await _context.Payments
                .Where(p => !p.IsFullyPaid)
                .SumAsync(p => p.RemainingBalance)
                .ConfigureAwait(false);
        }

        public async Task<string> GenerateReceiptNumberAsync()
        {
            var today = DateTime.Now;
            var prefix = $"RCP{today:yyyyMMdd}";

            // Get the highest receipt number for today
            var lastReceiptToday = await _context.Payments
                .Where(p => p.ReceiptNumber.StartsWith(prefix))
                .OrderByDescending(p => p.ReceiptNumber)
                .Select(p => p.ReceiptNumber)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            int sequenceNumber = 1;
            if (!string.IsNullOrEmpty(lastReceiptToday) && lastReceiptToday.Length > prefix.Length)
            {
                var sequencePart = lastReceiptToday.Substring(prefix.Length);
                if (int.TryParse(sequencePart, out int lastSequence))
                {
                    sequenceNumber = lastSequence + 1;
                }
            }

            return $"{prefix}{sequenceNumber:D4}";
        }
    }
}