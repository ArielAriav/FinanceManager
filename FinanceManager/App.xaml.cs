using FinanceManager.Services;
using FinanceManager.Pages;

namespace FinanceManager;

public partial class App : Application
{
    public App(LocalDbService db, MonthService monthService)
    {
        InitializeComponent();
        // Ensure DB is initialized before MainPage is shown
        MainPage = new LoadingPage(db, monthService);
    }
}

// Add this new page to show a loading indicator while initializing
public class LoadingPage : ContentPage
{
    public LoadingPage(LocalDbService db, MonthService monthService)
    {
        Content = new ActivityIndicator { IsRunning = true, VerticalOptions = LayoutOptions.CenterAndExpand };
        InitializeAsync(db, monthService);
    }

    private async void InitializeAsync(LocalDbService db, MonthService monthService)
    {
        await db.InitializeAsync();
        Application.Current!.MainPage = new AppShell();
    }
}

