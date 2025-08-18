using CommunityToolkit.Maui;
using FinanceManager.Helpers;
using FinanceManager.Pages;
using FinanceManager.Services;
using FinanceManager.ViewModels;
using System.Globalization;


namespace FinanceManager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var he = CultureInfo.GetCultureInfo("he-IL");
            CultureInfo.DefaultThreadCurrentCulture = he;
            CultureInfo.DefaultThreadCurrentUICulture = he;
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddSingleton<MonthService>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();


            // ViewModels
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<CategoryDetailsViewModel>();
            builder.Services.AddTransient<BudgetWizardViewModel>();
            builder.Services.AddTransient<AddEntryViewModel>();

            // Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<CategoryDetailsPage>();
            builder.Services.AddTransient<BudgetWizardPage>();
            builder.Services.AddTransient<AddEntryPage>();

            // Build and expose the ServiceProvider for ServiceHelper
            var app = builder.Build();
            ServiceHelper.Initialize(app.Services);

            return app;
        }
    }
}
