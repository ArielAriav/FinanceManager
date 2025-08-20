using FinanceManager.Helpers;
using FinanceManager.Models;
using FinanceManager.ViewModels;

namespace FinanceManager.Pages;

public partial class AddTransactionPage : ContentPage
{
	public AddTransactionPage()
	{
		InitializeComponent();

        var vm = ServiceHelper.Get<AddTransactionViewModel>();
        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AddTransactionViewModel vm)
            await vm.LoadAsync();
    }
}