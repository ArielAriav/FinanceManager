using FinanceManager.Models;
using FinanceManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace FinanceManager.ViewModels
{
    public class ManageCategoriesViewModel : INotifyPropertyChanged
    {
        private readonly LocalDbService _db;

        public ObservableCollection<Category> Items { get; } = new();
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private Category? _selected;
        public Category? Selected
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); LoadSelectedIntoForm(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand DeleteCommand { get; }

        public ManageCategoriesViewModel(LocalDbService db)
        {
            _db = db;

            LoadCommand = new Command(async () => await LoadAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            NewCommand = new Command(ClearForm);
            DeleteCommand = new Command<Category>(async c => await DeleteAsync(c));
        }

        public async Task LoadAsync()
        {
            Items.Clear();
            var all = await _db.GetAllAsync<Category>();
            foreach (var c in all.OrderBy(c => c.Name))
                Items.Add(c);
        }

        async Task SaveAsync()
        {
            var trimmed = Name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                await Shell.Current.DisplayAlert("שגיאה", "שם קטגוריה חובה", "אישור");
                return;
            }

            // Uniqueness check
            var exists = await _db.Conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Category WHERE Name = ? AND Id <> ?",
                trimmed, Selected?.Id ?? 0) > 0;
            if (exists)
            {
                await Shell.Current.DisplayAlert("שגיאה", "השם כבר קיים", "אישור");
                return;
            }

            if (Selected is null || Selected.Id == 0)
            {
                var c = new Category { Name = trimmed };
                await _db.InsertAsync(c);
            }
            else
            {
                Selected.Name = trimmed;
                await _db.UpdateAsync(Selected);
            }

            ClearForm();
            await LoadAsync();
        }

        async Task DeleteAsync(Category? c)
        {
            if (c is null) return;
            var action = await Shell.Current.DisplayActionSheet($"למחוק את {c.Name}?", "ביטול", "מחק");
            if (action == "מחק")
            {
                await _db.DeleteAsync(c);
                if (Selected?.Id == c.Id) ClearForm();
                await LoadAsync();
            }
        }

        void ClearForm()
        {
            Selected = null;
            Name = string.Empty;
        }

        void LoadSelectedIntoForm()
        {
            if (Selected is null) return;
            Name = Selected.Name;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string? n = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
