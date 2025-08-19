using FinanceManager.Models;
using FinanceManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Pages;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;

    // Prevents running the initial load more than once
    private bool _loadedOnce;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();

        // Store the injected ViewModel and bind it to the page
        _vm = vm;
        BindingContext = _vm;
    }

    // Runs when the page becomes visible for the first time
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Skip if we already performed the first load
        if (_loadedOnce) return;
        _loadedOnce = true;

        // Initial data load for the current month
        await _vm.LoadAsync();
    }

    // Runs every time navigation returns to this page
    // Good place to refresh data after closing a modal add screen
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Refresh data in case something changed while away
        await _vm.LoadAsync();
    }

    // Open the Add Expense modal page
    //private async void OnAddExpenseClicked(object sender, EventArgs e)
       // => await Navigation.PushModalAsync(new AddEntryPage(EntryType.Expense));

    private async void OnMyEnvelopesClicked(object sender, EventArgs e)
        => await Navigation.PushModalAsync(new AddEntryPage(EntryType.Expense));

    // Open the Add Income modal page
    private async void OnAddIncomeClicked(object sender, EventArgs e)
        => await Navigation.PushModalAsync(new AddEntryPage(EntryType.Income));
}
