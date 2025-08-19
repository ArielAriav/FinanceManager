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
        public SQLiteAsyncConnection Conn => _conn;
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
            await _conn.CreateTableAsync<Models.Envelope>();
            await _conn.CreateTableAsync<Models.EnvelopeEntry>();
            await _conn.ExecuteAsync("CREATE TABLE IF NOT EXISTS __Meta (Key TEXT PRIMARY KEY, Value TEXT)");

            await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_EnvelopeEntry_Env_YearMonth ON EnvelopeEntry(EnvelopeId, YearMonth)");
            await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_EnvelopeEntry_Env_Occurred ON EnvelopeEntry(EnvelopeId, OccurredAtUtc)");

            // Stability and basic rules
            await _conn.ExecuteAsync("PRAGMA foreign_keys=ON");
            var _ = await _conn.ExecuteScalarAsync<string>("PRAGMA journal_mode = WAL;");

            // Indexes for quick searches by month and time
            await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Transaction_YearMonth ON [Transaction](YearMonth)");
            await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Transaction_Category_YearMonth ON [Transaction](CategoryId, YearMonth)");
            await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS IX_Transaction_OccurredAtUtc ON [Transaction](OccurredAtUtc)");

            // One budget per category each month
            await _conn.ExecuteAsync("CREATE UNIQUE INDEX IF NOT EXISTS IX_Budget_Category_YearMonth ON [Budget](CategoryId, YearMonth)");
        }

        

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

    }

}
