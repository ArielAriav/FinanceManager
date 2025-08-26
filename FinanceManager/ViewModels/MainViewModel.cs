using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceManager.Models;
using FinanceManager.Services;
using Microsoft.Maui.Platform;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FinanceManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly LocalDbService _db;
    private readonly MonthService _month;
    static int ToYearMonth(DateTime d) => d.Year * 100 + d.Month;

    // The list fed into the CollectionView
    public ObservableCollection<CategoryRow> Transactions { get; } = new();

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

    public IAsyncRelayCommand ShowExpensesCommand { get; }
    public IAsyncRelayCommand ShowIncomeCommand { get; }
    public bool IsExpensesSelected => CurrentMode == EntryType.Expense;
    public bool IsIncomeSelected => CurrentMode == EntryType.Income;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsExpensesSelected))]
    [NotifyPropertyChangedFor(nameof(IsIncomeSelected))]
    private EntryType currentMode = EntryType.Expense;


    public MainViewModel(LocalDbService db, MonthService month)
    {
        _db = db;
        _month = month;

        ShowExpensesCommand = new AsyncRelayCommand(async () =>
        {
            CurrentMode = EntryType.Expense;   
            await LoadAsync();
        });

        ShowIncomeCommand = new AsyncRelayCommand(async () =>
        {
            CurrentMode = EntryType.Income;
            await LoadAsync();
        });
    }


    public async Task LoadAsync()
    {
        Transactions.Clear();

        var ym = ToYearMonth(CurrentMonth);
        var culture = CultureInfo.GetCultureInfo("he-IL");

        // Mapping categories
        var cats = await _db.GetAllAsync<Category>();
        var catMap = cats.ToDictionary(c => c.Id, c => c.Name);


        // Only the records for the month and type (expense/income) are brought in.
        var items = await _db.Conn.Table<Transaction>()
           .Where(t => t.YearMonth == ym && t.Type == currentMode)
           .ToListAsync();

        // Sum by category
        var sumsByCategory = items
            .GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Total)
            .ToList();

        // Monthly budgets – for expenses only
        Dictionary<int, decimal> budgets = new();
        if (currentMode == EntryType.Expense)
        {
            var monthBudgets = await _db.Conn.Table<Budget>()
                .Where(b => b.YearMonth == ym)
                .ToListAsync();

            budgets = monthBudgets.ToDictionary(b => b.CategoryId, b => b.MonthlyAmount);
        }

        var typeText = currentMode == EntryType.Expense ? "הוצאה" : "הכנסה";

        foreach (var s in sumsByCategory)
        {
            var name = catMap.TryGetValue(s.CategoryId, out var nm) ? nm : "קטגוריה";

            // Amount format: minus expense, plus income
            var signed = currentMode == EntryType.Expense ? -s.Total : s.Total;
            var amountText = string.Format(culture, "{0:C}", signed);

            string? budgetText = null;
            if (currentMode == EntryType.Expense && budgets.TryGetValue(s.CategoryId, out var budget))
            {
                budgetText = string.Format(culture, "{0:C} / {1:C}", s.Total, budget);
            }

            Transactions.Add(new CategoryRow(s.CategoryId, name, typeText, amountText, budgetText));
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

    [RelayCommand]
    private async Task OpenCategoryAsync(CategoryRow row)
    {
        // TODO: ניווט למסך פירוט לפי row.Id
        await Application.Current.MainPage.DisplayAlert(
            "קטגוריה", $"{row.CategoryName}", "סגור");
    }

}
