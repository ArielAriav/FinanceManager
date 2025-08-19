using FinanceManager.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services
{
    public class EnvelopeDbService
    {
        private readonly SQLiteAsyncConnection _conn;

        public EnvelopeDbService(SQLiteAsyncConnection conn)
        {
            _conn = conn;
        }

        // Retrieving all envelopes (including archived ones)
        public Task<List<Envelope>> GetAllEnvelopesAsync() =>
            _conn.Table<Envelope>().ToListAsync();

        // Retrieving active envelopes only
        public Task<List<Envelope>> GetActiveEnvelopesAsync() =>
            _conn.Table<Envelope>()
                 .Where(e => !e.IsArchived)
                 .OrderBy(e => e.Name)
                 .ToListAsync();

        // Retrieving all transactions for a specific envelope (by ID)
        public Task<List<EnvelopeEntry>> GetEnvelopeEntriesAsync(int envelopeId) =>
            _conn.Table<EnvelopeEntry>()
                 .Where(e => e.EnvelopeId == envelopeId)
                 .OrderByDescending(e => e.OccurredAtUtc)
                 .ToListAsync();

        public async Task<int> AddEnvelopeEntryAsync(EnvelopeEntry entry)
        {
            DateTime local = entry.OccurredAtUtc.Kind switch
            {
                DateTimeKind.Utc => entry.OccurredAtUtc.ToLocalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(entry.OccurredAtUtc, DateTimeKind.Local),
                DateTimeKind.Local => entry.OccurredAtUtc
            };

            var localMidnight = new DateTime(local.Year, local.Month, local.Day, 0, 0, 0, DateTimeKind.Local);
            entry.OccurredAtUtc = localMidnight.ToUniversalTime();
            entry.YearMonth = local.Year * 100 + local.Month;

            return await _conn.InsertAsync(entry);
        }

        public async Task<decimal> GetEnvelopeBalanceAsync(int envelopeId)
        {
            var sql = "SELECT IFNULL(SUM(Amount), 0) FROM EnvelopeEntry WHERE EnvelopeId = ?";
            return await _conn.ExecuteScalarAsync<decimal>(sql, envelopeId);
        }

        public async Task<decimal> GetEnvelopeExpensesOnlyAsync(int envelopeId)
        {
            var sql = "SELECT IFNULL(SUM(Amount), 0) FROM EnvelopeEntry WHERE EnvelopeId = ? AND Type = ?";
            return await _conn.ExecuteScalarAsync<decimal>(sql, envelopeId, EntryType.Expense);
        }

        public async Task<decimal> GetEnvelopeIncomeOnlyAsync(int envelopeId)
        {
            var sql = "SELECT IFNULL(SUM(Amount), 0) FROM EnvelopeEntry WHERE EnvelopeId = ? AND Type = ?";
            return await _conn.ExecuteScalarAsync<decimal>(sql, envelopeId, EntryType.Income);
        }

    }
}

