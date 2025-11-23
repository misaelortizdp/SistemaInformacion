using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    async Task Register()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Por favor complete todos los campos";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Las contraseñas no coinciden";
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "La contraseña debe tener al menos 6 caracteres";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var success = await _authService.RegisterAsync(Username, Email, Password);

            if (success)
            {
                await Shell.Current.GoToAsync("//dashboard");
            }
            else
            {
                ErrorMessage = "Error al registrar usuario";
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
    async Task GoToLogin()
    {
        await Shell.Current.GoToAsync("//login");
    }
}
