using CommunityToolkit.Mvvm.ComponentModel;
using FinanceManager.Models;
using FinanceManager.Services;
using System.Collections.ObjectModel;

namespace FinanceManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly LocalDbService _db;
    private readonly MonthService _month;

    public ObservableCollection<TransactionRow> Transactions { get; } = new();

    public MainViewModel(LocalDbService db, MonthService month)
    {
        _db = db;
        _month = month;
    }

   
    public record TransactionRow(
        int Id,
        string TypeText,
        string CategoryName,
        string AmountText,
        string WhenText
    );

    public async Task LoadAsync()
    {
        Transactions.Clear();

        // נטען כל הקטגוריות למפה מהירה
        var cats = await _db.GetAllAsync<Category>();
        var catMap = cats.ToDictionary(c => c.Id, c => c.Name);

        // כל התנועות, ממוין לפי תאריך יורד
        var all = await _db.GetAllAsync<Transaction>();
        foreach (var t in all.OrderByDescending(t => t.OccurredAtUtc))
        {
            var name = catMap.TryGetValue(t.CategoryId, out var nm) ? nm : "קטגוריה";
            var typeText = t.Type == EntryType.Expense ? "הוצאה" : "הכנסה";
            var amountText = string.Format("{0:C}", t.Amount * (t.Type == EntryType.Expense ? -1 : 1));
            var whenText = t.OccurredAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

            Transactions.Add(new TransactionRow(
                t.Id,
                typeText,
                name,
                amountText,
                whenText
            ));
        }
    }
}
