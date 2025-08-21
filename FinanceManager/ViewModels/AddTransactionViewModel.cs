using FinanceManager.Models;
using FinanceManager.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FinanceManager.ViewModels;

public class AddTransactionViewModel : INotifyPropertyChanged
{
    private readonly LocalDbService _db;
    private readonly TransactionDbService _txService;
    private readonly MonthService _month;

    private bool _isDropdownOpen;
    public bool IsDropdownOpen
    {
        get => _isDropdownOpen;
        set { _isDropdownOpen = value; OnPropertyChanged(); }
    }

    public bool HasCategories => Categories.Any();
    public bool NoCategories => !HasCategories;
    public ICommand ToggleDropdownCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public DateTime MonthStart => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    public DateTime MonthEnd => MonthStart.AddMonths(1).AddDays(-1);

    public ObservableCollection<Category> Categories { get; } = new();

    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory == value) return;
            _selectedCategory = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedCategoryDisplay));
            IsDropdownOpen = false; // Closed after selection
        }
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

    private DateTime _date = DateTime.Today;
    public DateTime Date
    {
        get => _date;
        set { _date = value.Date; OnPropertyChanged(); }
    }

    public EntryType EntryType { get; private set; } = EntryType.Expense;
    public string SelectedCategoryDisplay => SelectedCategory?.Name ?? "בחר קטגוריה";


    public AddTransactionViewModel(LocalDbService db, MonthService month)
    {
        _db = db;
        _month = month;
        _txService = new TransactionDbService(db.Conn);

        // When the category collection changes, update the HasCategories and NoCategories bindings
        Categories.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(HasCategories));
            OnPropertyChanged(nameof(NoCategories));
        };

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await CancelAsync());
        ToggleDropdownCommand = new Command(() => IsDropdownOpen = !IsDropdownOpen);

    }

    public void SetType(EntryType type) => EntryType = type;

    public async Task LoadAsync()
    {
        var all = await _db.GetAllAsync<Category>();

        Categories.Clear();
        foreach (var c in all.OrderBy(c => c.Name))
            Categories.Add(c);

        SelectedCategory = null;

        // Update the Notifications and Picker UI
        OnPropertyChanged(nameof(HasCategories));
        OnPropertyChanged(nameof(NoCategories));
        OnPropertyChanged(nameof(SelectedCategoryDisplay));
    }


    public async Task SaveAsync()
    {
        if (SelectedCategory is null)
        {
            await Application.Current!.MainPage!.DisplayAlert("שגיאה", "בחרי קטגוריה", "אישור");
            return;
        }

        if (!decimal.TryParse(AmountText, out var amount) || amount <= 0)
        {
            await Application.Current!.MainPage!.DisplayAlert("שגיאה", "הזיני סכום תקין", "אישור");
            return;
        }

        if (Date.Date > DateTime.Today)
        {
            await Application.Current!.MainPage!.DisplayAlert("שגיאה", "אי אפשר לבחור תאריך עתידי", "אישור");
            return;
        }

        // אם אתחול חודש חשוב לצד תקציבים, נשאיר את הקריאה בלי להשתמש בתוצאה
        _ = await _month.EnsureMonthInitializedAsync();

        var localMidnight = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0, DateTimeKind.Local);

        var tx = new Transaction
        {
            CategoryId = SelectedCategory.Id,
            Type = EntryType,
            Amount = Math.Abs(amount),
            OccurredAtUtc = localMidnight,
            Note = string.IsNullOrWhiteSpace(Note) ? null : Note!.Trim()
        };

        await _txService.AddTransactionAsync(tx);

        await Application.Current!.MainPage!.DisplayAlert("נשמר", "התנועה נשמרה", "אישור");
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

    public async Task CancelAsync() =>
        await Application.Current!.MainPage!.Navigation.PopModalAsync();

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
