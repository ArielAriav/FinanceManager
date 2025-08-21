using FinanceManager.ViewModels;
using FinanceManager.Helpers;

namespace FinanceManager.Pages;

public partial class ManageCategoriesPage : ContentPage
{
    public ManageCategoriesPage()
    {
        InitializeComponent();
        var vm = ServiceHelper.Get<ManageCategoriesViewModel>();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ManageCategoriesViewModel vm)
            await vm.LoadAsync();
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//main");
    }
}
