using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceManager.Models;
using FinanceManager.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FinanceManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly LocalDbService _db;
    private readonly MonthService _month;
    static int ToYearMonth(DateTime d) => d.Year * 100 + d.Month;

    public ObservableCollection<TransactionRow> Transactions { get; } = new();

    // Beginning of the current month
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayMonth))]
    [NotifyPropertyChangedFor(nameof(CanGoNext))]
    private DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

    // Hebrew month name
    public string DisplayMonth => currentMonth.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("he-IL"));

    // Not progressing beyond the current month
    public bool CanGoNext =>
        currentMonth.AddMonths(1) <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

    public MainViewModel(LocalDbService db, MonthService month)
    {
        _db = db;
        _month = month;
    }

    public record TransactionRow(int Id, string TypeText, string CategoryName, string AmountText, string WhenText);

    public async Task LoadAsync()
    {
        Transactions.Clear();

        var ym = ToYearMonth(CurrentMonth);

        // Mapping categories
        var cats = await _db.GetAllAsync<Category>();
        var catMap = cats.ToDictionary(c => c.Id, c => c.Name);

        // Effective pumping by month
        var list = await _db.Conn.Table<Transaction>()
                        .Where(t => t.YearMonth == ym)
                        .OrderByDescending(t => t.OccurredAtUtc)
                        .ToListAsync();

        foreach (var t in list)
        {
            var name = catMap.TryGetValue(t.CategoryId, out var nm) ? nm : "קטגוריה";
            var typeText = t.Type == EntryType.Expense ? "הוצאה" : "הכנסה";
            var signed = t.Type == EntryType.Expense ? -t.Amount : t.Amount;
            var amountText = string.Format(CultureInfo.CurrentCulture, "{0:C}", signed);
            var whenText = t.OccurredAtUtc.ToLocalTime().ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);

            Transactions.Add(new TransactionRow(t.Id, typeText, name, amountText, whenText));
        }
    }


    [RelayCommand]
    private async Task PrevMonthAsync()
    {
        CurrentMonth = currentMonth.AddMonths(-1); 
        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextMonthAsync()
    {
        if (!CanGoNext) return;
        CurrentMonth = currentMonth.AddMonths(1);
        await LoadAsync();
    }
}
