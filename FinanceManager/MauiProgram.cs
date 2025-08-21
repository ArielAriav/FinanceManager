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
                    fonts.AddFont("Rubik-Regular.ttf", "Rubik");
                    fonts.AddFont("Rubik-Medium.ttf", "RubikMedium");
                    fonts.AddFont("Rubik-Bold.ttf", "RubikBold");
                });

            // Services
            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddSingleton<MonthService>();
            builder.Services.AddSingleton<EnvelopeDbService>();
            builder.Services.AddSingleton<TransactionDbService>();
            builder.Services.AddSingleton<BudgetDbService>();


            // ViewModels
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<ManageCategoriesViewModel>();
            builder.Services.AddTransient<BudgetWizardViewModel>();
            builder.Services.AddTransient<AddTransactionViewModel>();
            builder.Services.AddTransient<MyEnvelopesViewModel>();

            // Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<BudgetWizardPage>();
            builder.Services.AddTransient<AddTransactionPage>();
            builder.Services.AddTransient<MyEnvelopesPage>();
            builder.Services.AddTransient<ManageCategoriesPage>();

            // Build and expose the ServiceProvider for ServiceHelper
            var app = builder.Build();
            ServiceHelper.Initialize(app.Services);

            return app;
        }
    }
}
