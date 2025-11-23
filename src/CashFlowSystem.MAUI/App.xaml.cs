using CashFlowSystem.MAUI.Services;

namespace CashFlowSystem.MAUI;

public partial class App : Application
{
    private readonly IAuthService _authService;

    public App(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // Check if user is already logged in
        var isAuthenticated = await _authService.IsAuthenticatedAsync();
        if (isAuthenticated)
        {
            await Shell.Current.GoToAsync("//dashboard");
        }
        else
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
