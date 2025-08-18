using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class Budget
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [Indexed] public int CategoryId { get; set; }
        [Indexed] public int YearMonth { get; set; } // למשל 202508
        [NotNull] public decimal MonthlyAmount { get; set; }
    }
}
