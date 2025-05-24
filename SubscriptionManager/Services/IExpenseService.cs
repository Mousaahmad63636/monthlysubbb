using SubscriptionManager.Models;

namespace SubscriptionManager.Services
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllExpensesAsync();
        Task<Expense> GetExpenseByIdAsync(int id);
        Task<Expense> AddExpenseAsync(Expense expense);
        Task<Expense> UpdateExpenseAsync(Expense expense);
        Task DeleteExpenseAsync(int id);
        Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalExpensesAsync(DateTime startDate, DateTime endDate);
        Task<List<Expense>> GetExpensesByMonthAsync(int year, int month);
        Task<decimal> GetTotalExpensesForMonthAsync(int year, int month);
        Task<decimal> GetTotalConsumptionForMonthAsync(int year, int month);
        Task<decimal> GetTotalRevenueForMonthAsync(int year, int month);
        Task<decimal> GetTotalProfitForMonthAsync(int year, int month);
    }
}