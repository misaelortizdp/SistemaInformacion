using CashFlowSystem.MAUI.Services;
using CashFlowSystem.MAUI.ViewModels;
using CashFlowSystem.MAUI.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace CashFlowSystem.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IStorageService, StorageService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<TransactionsViewModel>();
        builder.Services.AddTransient<AddTransactionViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();
        builder.Services.AddTransient<CashRegisterViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<TransactionsPage>();
        builder.Services.AddTransient<AddTransactionPage>();
        builder.Services.AddTransient<ReportsPage>();
        builder.Services.AddTransient<CashRegisterPage>();

        return builder.Build();
    }
}
