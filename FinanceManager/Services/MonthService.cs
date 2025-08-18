using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Models;


namespace FinanceManager.Services
{
    public class MonthService
    {
        private readonly LocalDbService _db;
        public MonthService(LocalDbService db) => _db = db;

        public static int ToYearMonth(DateTime dtLocal)
            => dtLocal.Year * 100 + dtLocal.Month;

        public async Task<int> EnsureMonthInitializedAsync()
        {
            var nowLocal = DateTime.Now;
            var currYm = ToYearMonth(nowLocal);

            var lastYmStr = await _db.GetMetaAsync("last_initialized_yearmonth");
            if (int.TryParse(lastYmStr, out var lastYm) && lastYm == currYm)
                return currYm;

            // Bring all the categories
            var categories = await _db.GetAllAsync<Category>();

            // Initialize budget for current month based on previous month if it exists
            var prevYm = ToYearMonth(nowLocal.AddMonths(-1));

            foreach (var c in categories)
            {
                var prev = await _db.GetBudgetAsync(c.Id, prevYm);
                var amount = prev?.MonthlyAmount ?? 0m;
                var exists = await _db.GetBudgetAsync(c.Id, currYm);
                if (exists is null)
                {
                    await _db.InsertAsync(new Budget
                    {
                        CategoryId = c.Id,
                        YearMonth = currYm,
                        MonthlyAmount = amount
                    });
                }
            }

            await _db.SetMetaAsync("last_initialized_yearmonth", currYm.ToString());
            return currYm;
        }
    }
}
