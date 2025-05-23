using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Models;  // Changed
using SubscriptionManager.Data;
namespace SubscriptionManager.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> GetAllExpensesAsync()
        {
            return await _context.Expenses
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<Expense> GetExpenseByIdAsync(int id)
        {
            return await _context.Expenses.FindAsync(id);
        }

        public async Task<Expense> AddExpenseAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Expenses
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalExpensesAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Expenses
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .SumAsync(e => e.Amount);
        }
    }
}