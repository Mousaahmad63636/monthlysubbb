using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Models;
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
                .AsNoTracking() 
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Expense> GetExpenseByIdAsync(int id)
        {
            var expense = await _context.Expenses
                .FindAsync(id)
                .ConfigureAwait(false);

            if (expense == null)
                throw new InvalidOperationException($"Expense with ID {id} not found.");

            return expense;
        }

        public async Task<Expense> AddExpenseAsync(Expense expense)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense));

            expense.CreatedAt = DateTime.Now;
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(Expense expense)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense));

            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return expense;
        }

        public async Task DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses
                .FindAsync(id)
                .ConfigureAwait(false);

            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
           
            var endDateInclusive = endDate.Date.AddDays(1).AddTicks(-1);

            return await _context.Expenses
                .Where(e => e.Date >= startDate.Date && e.Date <= endDateInclusive)
                .OrderByDescending(e => e.Date)
                .AsNoTracking() 
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<decimal> GetTotalExpensesAsync(DateTime startDate, DateTime endDate)
        {
         
            var endDateInclusive = endDate.Date.AddDays(1).AddTicks(-1);

            return await _context.Expenses
                .Where(e => e.Date >= startDate.Date && e.Date <= endDateInclusive)
                .SumAsync(e => e.Amount)
                .ConfigureAwait(false);
        }
    }
}