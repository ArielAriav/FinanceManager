using FinanceManager.Helpers; 
using FinanceManager.Models;
using FinanceManager.ViewModels;

namespace FinanceManager.Pages;

public partial class AddEntryPage : ContentPage
{
    public AddEntryPage()
    {
        InitializeComponent();

        var vm = ServiceHelper.Get<AddEntryViewModel>();
        BindingContext = vm;
    }

    public AddEntryPage(EntryType type) : this()
    {
        if (BindingContext is AddEntryViewModel vm)
            vm.SetType(type);
    }
}
