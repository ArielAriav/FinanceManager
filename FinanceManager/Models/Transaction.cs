using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models;

public enum EntryType { Expense = 0, Income = 1 }

public class Transaction
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public int CategoryId { get; set; }
    [Indexed] public int YearMonth { get; set; }     // 202508
    [NotNull] public EntryType Type { get; set; }    // הוצאה או הכנסה
    [NotNull] public decimal Amount { get; set; }    // חיובי תמיד
    [NotNull] public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
}

