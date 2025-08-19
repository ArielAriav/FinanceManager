using FinanceManager.Helpers;
using FinanceManager.Models;
using FinanceManager.ViewModels;


namespace FinanceManager.Pages;

public partial class MyEnvelopesPage : ContentPage
{
	public MyEnvelopesPage()
	{
		InitializeComponent();

        var vm = ServiceHelper.Get<MyEnvelopesViewModel>();
        BindingContext = vm;
    }
}