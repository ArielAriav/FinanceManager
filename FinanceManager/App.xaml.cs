using FinanceManager.Services;
using FinanceManager.Pages;

namespace FinanceManager;

public partial class App : Application
{
    public App(LocalDbService db, MonthService monthService)
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}

