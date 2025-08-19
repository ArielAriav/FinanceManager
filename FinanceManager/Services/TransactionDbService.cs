using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Models;

namespace FinanceManager.Services
{
    public class TransactionDbService
    {
        private static int ToYearMonth(DateTime local) => local.Year * 100 + local.Month;
        private readonly SQLiteAsyncConnection _conn;
        public TransactionDbService(SQLiteAsyncConnection conn)
        {
            _conn = conn;
        }

        public async Task<int> AddTransactionAsync(Models.Transaction t)
        {
            // Make sure that the date is entered as a local date.
            DateTime local = t.OccurredAtUtc.Kind switch
            {
                DateTimeKind.Utc => t.OccurredAtUtc.ToLocalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(t.OccurredAtUtc, DateTimeKind.Local),
                DateTimeKind.Local => t.OccurredAtUtc
            };


            // Date only, time 00:00 local time
            var localMidnight = new DateTime(local.Year, local.Month, local.Day, 0, 0, 0, DateTimeKind.Local);

            // Store in UTC, and YearMonth will be calculated according to local
            t.OccurredAtUtc = localMidnight.ToUniversalTime();
            t.YearMonth = ToYearMonth(localMidnight);

            return await _conn.InsertAsync(t);
        }


        // List of transactions by category and by month
        public Task<List<Models.Transaction>> GetTransactionsAsync(int categoryId, int yearMonth) =>
            _conn.Table<Models.Transaction>()
                .Where(t => t.CategoryId == categoryId && t.YearMonth == yearMonth)
                .OrderByDescending(t => t.OccurredAtUtc)
                .ToListAsync();


        // Amount of expenses for a given month by category
        public async Task<decimal> SumExpensesAsync(int categoryId, int yearMonth)
        {
            // We will only take Type = Expense
            var sql = "SELECT IFNULL(SUM(Amount),0) FROM [Transaction] WHERE CategoryId=? AND YearMonth=? AND Type=?";
            return await _conn.ExecuteScalarAsync<decimal>(sql, categoryId, yearMonth, Models.EntryType.Expense);
        }

    }
}
