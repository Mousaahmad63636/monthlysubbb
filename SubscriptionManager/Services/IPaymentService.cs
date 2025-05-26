using SubscriptionManager.Models;

namespace SubscriptionManager.Services
{
    public interface IPaymentService
    {
        /// <summary>
        /// Gets all payments for a specific customer
        /// </summary>
        /// <param name="customerId">The customer's ID</param>
        /// <returns>List of payments for the customer</returns>
        Task<List<Payment>> GetCustomerPaymentsAsync(int customerId);

        /// <summary>
        /// Gets a payment by its ID
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>The payment record</returns>
        Task<Payment> GetPaymentByIdAsync(int id);

        /// <summary>
        /// Creates a new payment record for settling a customer's current bill
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="amountPaid">Amount being paid</param>
        /// <param name="paymentMethod">Method of payment (Cash, Card, Transfer, etc.)</param>
        /// <param name="notes">Optional payment notes</param>
        /// <returns>The created payment record</returns>
        Task<Payment> SettleCurrentBillAsync(int customerId, decimal amountPaid, string paymentMethod, string notes = "");

        /// <summary>
        /// Adds a new payment record
        /// </summary>
        /// <param name="payment">Payment to add</param>
        /// <returns>The created payment</returns>
        Task<Payment> AddPaymentAsync(Payment payment);

        /// <summary>
        /// Updates an existing payment record
        /// </summary>
        /// <param name="payment">Payment to update</param>
        /// <returns>The updated payment</returns>
        Task<Payment> UpdatePaymentAsync(Payment payment);

        /// <summary>
        /// Deletes a payment record
        /// </summary>
        /// <param name="id">Payment ID to delete</param>
        Task DeletePaymentAsync(int id);

        /// <summary>
        /// Gets the customer's current outstanding bill details
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Payment object representing the current bill</returns>
        Task<Payment?> GetCurrentBillAsync(int customerId);

        /// <summary>
        /// Gets all pending payments across all customers
        /// </summary>
        /// <returns>List of pending payments</returns>
        Task<List<Payment>> GetPendingPaymentsAsync();

        /// <summary>
        /// Gets payment statistics for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Payment statistics</returns>
        Task<PaymentStatistics> GetCustomerPaymentStatisticsAsync(int customerId);

        /// <summary>
        /// Gets total outstanding balance for all customers
        /// </summary>
        /// <returns>Total outstanding balance</returns>
        Task<decimal> GetTotalOutstandingBalanceAsync();

        /// <summary>
        /// Generates a unique receipt number
        /// </summary>
        /// <returns>Unique receipt number</returns>
        Task<string> GenerateReceiptNumberAsync();
    }

    /// <summary>
    /// Payment statistics for a customer
    /// </summary>
    public class PaymentStatistics
    {
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int TotalPayments { get; set; }
        public int PendingPayments { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal AveragePaymentAmount { get; set; }
    }
}