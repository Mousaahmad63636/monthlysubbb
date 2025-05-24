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
            // Include the entire end date (up to 23:59:59.999)
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
            // Include the entire end date (up to 23:59:59.999)
            var endDateInclusive = endDate.Date.AddDays(1).AddTicks(-1);

            return await _context.Expenses
                .Where(e => e.Date >= startDate.Date && e.Date <= endDateInclusive)
                .SumAsync(e => e.Amount)
                .ConfigureAwait(false);
        }

        public async Task<List<Expense>> GetExpensesByMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await GetExpensesByDateRangeAsync(startDate, endDate)
                .ConfigureAwait(false);
        }

        public async Task<decimal> GetTotalExpensesForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await GetTotalExpensesAsync(startDate, endDate)
                .ConfigureAwait(false);
        }

        public async Task<decimal> GetTotalConsumptionForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var endDateInclusive = endDate.Date.AddDays(1).AddTicks(-1);

            try
            {
                // Calculate total consumption directly in the database query
                // UnitsUsed = NewCounter - OldCounter
                var totalConsumption = await _context.CounterHistories
                    .Where(ch => ch.RecordDate >= startDate && ch.RecordDate <= endDateInclusive)
                    .SumAsync(ch => ch.NewCounter - ch.OldCounter)
                    .ConfigureAwait(false);

                return totalConsumption;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error calculating consumption: {ex.Message}");
                return 0;
            }
        }

        public async Task<decimal> GetTotalRevenueForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var endDateInclusive = endDate.Date.AddDays(1).AddTicks(-1);

            try
            {
                // Calculate revenue from usage billing (counter histories)
                var usageRevenue = await _context.CounterHistories
                    .Where(ch => ch.RecordDate >= startDate && ch.RecordDate <= endDateInclusive)
                    .SumAsync(ch => ch.BillAmount)
                    .ConfigureAwait(false);

                // Calculate revenue from monthly subscription fees
                // This is an approximation - getting active customers and their monthly fees
                var subscriptionRevenue = await _context.CustomerSubscriptions
                    .Where(cs => cs.IsActive)
                    .SumAsync(cs => cs.MonthlySubscriptionFee)
                    .ConfigureAwait(false);

                return usageRevenue + subscriptionRevenue;
            }
            catch (Exception)
            {
                // If there's an error calculating revenue, return 0
                return 0;
            }
        }

        public async Task<decimal> GetTotalProfitForMonthAsync(int year, int month)
        {
            try
            {
                // Get total revenue for the month
                var totalRevenue = await GetTotalRevenueForMonthAsync(year, month)
                    .ConfigureAwait(false);

                // Get total expenses for the month
                var totalExpenses = await GetTotalExpensesForMonthAsync(year, month)
                    .ConfigureAwait(false);

                // Profit = Revenue - Expenses
                return totalRevenue - totalExpenses;
            }
            catch (Exception)
            {
                // If there's an error calculating profit, return 0
                return 0;
            }
        }

        // TEMPORARY DEBUGGING METHOD - Remove after troubleshooting
        public async Task<string> DebugConsumptionDataAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var endDateInclusive = endDate.Date.AddDays(1).AddTicks(-1);

            try
            {
                var records = await _context.CounterHistories
                    .Where(ch => ch.RecordDate >= startDate && ch.RecordDate <= endDateInclusive)
                    .Select(ch => new
                    {
                        ch.Id,
                        ch.RecordDate,
                        ch.OldCounter,
                        ch.NewCounter,
                        UnitsUsed = ch.NewCounter - ch.OldCounter
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);

                var totalRecords = await _context.CounterHistories.CountAsync();

                return $"Month: {year}-{month}\n" +
                       $"Date range: {startDate:yyyy-MM-dd} to {endDateInclusive:yyyy-MM-dd}\n" +
                       $"Total CounterHistory records in DB: {totalRecords}\n" +
                       $"Records in date range: {records.Count}\n" +
                       $"Records detail:\n" +
                       string.Join("\n", records.Select(r =>
                           $"  ID:{r.Id} Date:{r.RecordDate:yyyy-MM-dd} Old:{r.OldCounter} New:{r.NewCounter} Used:{r.UnitsUsed}"));
            }
            catch (Exception ex)
            {
                return $"Debug error: {ex.Message}";
            }
        }
    }
}