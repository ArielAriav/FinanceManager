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
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    // Open the My Envelopes page
    private async void OnMyEnvelopesClicked(object sender, EventArgs e)
    {
        var page = new MyEnvelopesPage();
        page.Disappearing += OnModalClosedRefresh;
        await Navigation.PushModalAsync(page);
    }

    // Open the Add Transaction page
    private async void OnAddTransactionClicked(object sender, EventArgs e)
    {
        var page = new AddTransactionPage();

        // Refresh when the add screen closes
        page.Disappearing += OnModalClosedRefresh;

        await Navigation.PushModalAsync(page);
    }
    // Open the Category Management page
    private async void OnManageCategoriesClicked(object sender, EventArgs e)
    {
        var page = new ManageCategoriesPage();
        page.Disappearing += OnModalClosedRefresh;
        await Navigation.PushModalAsync(page);
    }

    // Open Budget Management page
    private async void OnManageBudgetClicked(object sender, EventArgs e)
    {
        var page = new ManageBudgetPage();
        page.Disappearing += OnModalClosedRefresh;
        await Navigation.PushModalAsync(page);
    }

    // Shared handler: disconnects to prevent leakage, then reloads
    private async void OnModalClosedRefresh(object? sender, EventArgs e)
    {
        if (sender is Page p)
            p.Disappearing -= OnModalClosedRefresh;

        await _vm.LoadAsync();
    }
}
