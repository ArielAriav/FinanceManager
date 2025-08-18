using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FinanceManager.Models;
using FinanceManager.Services;

namespace FinanceManager.ViewModels;

public class AddEntryViewModel : INotifyPropertyChanged
{
    private readonly LocalDbService _db;
    private readonly MonthService _month;

    public ObservableCollection<Category> Categories { get; } = new();

    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set { _selectedCategory = value; OnPropertyChanged(); }
    }

    private string _amountText = string.Empty;
    public string AmountText
    {
        get => _amountText;
        set { _amountText = value; OnPropertyChanged(); }
    }

    private string? _note;
    public string? Note
    {
        get => _note;
        set { _note = value; OnPropertyChanged(); }
    }

    public EntryType EntryType { get; private set; } = EntryType.Expense;

    public AddEntryViewModel(LocalDbService db, MonthService month)
    {
        _db = db;
        _month = month;
    }

    public void SetType(EntryType type) => EntryType = type;

    public async Task LoadAsync()
    {
        if (Categories.Count > 0) return;
        Categories.Clear();
        var cats = await _db.GetAllAsync<Category>();
        foreach (var c in cats) Categories.Add(c);
        SelectedCategory ??= Categories.FirstOrDefault();
    }

    public async Task SaveAsync()
    {
        if (SelectedCategory is null)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", "Choose category", "OK");
            return;
        }
        if (!decimal.TryParse(AmountText, out var amount) || amount <= 0)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", "Enter valid amount", "OK");
            return;
        }

        var ym = await _month.EnsureMonthInitializedAsync();
        var tx = new Transaction
        {
            CategoryId = SelectedCategory.Id,
            YearMonth = ym,
            Type = EntryType,
            Amount = Math.Abs(amount),
            OccurredAtUtc = DateTime.UtcNow,
            Note = string.IsNullOrWhiteSpace(Note) ? null : Note!.Trim()
        };
        await _db.InsertAsync(tx);
        await Application.Current!.MainPage!.DisplayAlert("Saved", "Entry saved", "OK");
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    public async Task CancelAsync() =>
        await Application.Current!.MainPage!.Navigation.PopModalAsync();

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
