using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services
{
    public class LocalDbService
    {
        private const string DbName = "finance.db";
        private readonly SQLiteAsyncConnection _conn;

        public LocalDbService()
        {
            SQLitePCL.Batteries_V2.Init();
            var path = Path.Combine(FileSystem.AppDataDirectory, DbName);
            _conn = new SQLiteAsyncConnection(path);
        }

        public async Task InitializeAsync()
        {
            await _conn.CreateTableAsync<Models.Category>();
            await _conn.CreateTableAsync<Models.Budget>();
            await _conn.CreateTableAsync<Models.Transaction>();
            await _conn.ExecuteAsync("CREATE TABLE IF NOT EXISTS __Meta (Key TEXT PRIMARY KEY, Value TEXT)");
        }

        public SQLiteAsyncConnection Conn => _conn;

        // Meta
        public Task<string?> GetMetaAsync(string key) =>
            _conn.ExecuteScalarAsync<string?>("SELECT Value FROM __Meta WHERE Key = ?", key);

        public Task<int> SetMetaAsync(string key, string value) =>
            _conn.ExecuteAsync("INSERT OR REPLACE INTO __Meta(Key,Value) VALUES(?,?)", key, value);

        // CRUD rules
        public Task<int> InsertAsync<T>(T entity) where T : new() => _conn.InsertAsync(entity);
        public Task<int> UpdateAsync<T>(T entity) where T : new() => _conn.UpdateAsync(entity);
        public Task<int> DeleteAsync<T>(T entity) where T : new() => _conn.DeleteAsync(entity);
        public Task<List<T>> GetAllAsync<T>() where T : new() => _conn.Table<T>().ToListAsync();

        // Amount of expenses for a given month by category
        public async Task<decimal> SumExpensesAsync(int categoryId, int yearMonth)
        {
            // We will only take Type = Expense
            var sql = "SELECT IFNULL(SUM(Amount),0) FROM [Transaction] WHERE CategoryId=? AND YearMonth=? AND Type=?";
            return await _conn.ExecuteScalarAsync<decimal>(sql, categoryId, yearMonth, Models.EntryType.Expense);
        }

        // List of transactions by category and by month
        public Task<List<Models.Transaction>> GetTransactionsAsync(int categoryId, int yearMonth) =>
            _conn.Table<Models.Transaction>()
                .Where(t => t.CategoryId == categoryId && t.YearMonth == yearMonth)
                .OrderByDescending(t => t.OccurredAtUtc)
                .ToListAsync();

        // Active budget per category and month
        public async Task<Models.Budget?> GetBudgetAsync(int categoryId, int yearMonth)
        {
            var result = await _conn.Table<Models.Budget>()
                .Where(b => b.CategoryId == categoryId && b.YearMonth == yearMonth)
                .FirstOrDefaultAsync();

            return result; // can be budget or null if not found
        }
    }
}
