using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    async Task Login()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Por favor ingrese email y contraseña";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var success = await _authService.LoginAsync(Email, Password);

            if (success)
            {
                await Shell.Current.GoToAsync("//dashboard");
            }
            else
            {
                ErrorMessage = "Email o contraseña incorrectos";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task GoToRegister()
    {
        await Shell.Current.GoToAsync("//register");
    }
}
