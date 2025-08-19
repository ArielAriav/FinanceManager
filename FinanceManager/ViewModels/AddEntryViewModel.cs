using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FinanceManager.Models;
using FinanceManager.Services;
using System.Globalization; 

namespace FinanceManager.ViewModels;

public class AddEntryViewModel : INotifyPropertyChanged
{
    private readonly LocalDbService _db;
    private readonly TransactionDbService _txService;
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

    private DateTime _date = DateTime.Today;
    public DateTime Date
    {
        get => _date;
        set { _date = value.Date; OnPropertyChanged(); }
    }

    public EntryType EntryType { get; private set; } = EntryType.Expense;

    public AddEntryViewModel(LocalDbService db, MonthService month)
    {
        _db = db;
        _month = month;
        _txService = new TransactionDbService(db.Conn);
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
