using FinanceManager.Models;
using FinanceManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Pages;

public partial class MainPage : ContentPage
{
    private MainViewModel? _vm;
    private bool _initialized;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (_initialized) return;
        if (Handler?.MauiContext is null) return;

        _vm = Handler.MauiContext.Services.GetRequiredService<MainViewModel>();
        BindingContext = _vm;
        _initialized = true;
        await _vm.LoadAsync();
    }

    private async void OnAddExpenseClicked(object sender, EventArgs e)
        => await Navigation.PushModalAsync(new AddEntryPage(EntryType.Expense));

    private async void OnAddIncomeClicked(object sender, EventArgs e)
        => await Navigation.PushModalAsync(new AddEntryPage(EntryType.Income));
}
