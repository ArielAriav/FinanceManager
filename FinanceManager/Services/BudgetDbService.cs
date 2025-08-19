using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services
{
    public class BudgetDbService
    {
        private readonly SQLiteAsyncConnection _conn;
        public BudgetDbService(SQLiteAsyncConnection conn)
        {
            _conn = conn;
        }

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
