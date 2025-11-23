using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class CashRegisterViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private bool hasCashRegister;

    [ObservableProperty]
    private Guid? cashRegisterId;

    [ObservableProperty]
    private decimal openingBalance;

    [ObservableProperty]
    private decimal closingBalance;

    [ObservableProperty]
    private DateTime? openingDate;

    [ObservableProperty]
    private bool isBusy;

    public CashRegisterViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
    }

    [RelayCommand]
    async Task LoadCashRegister()
    {
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            var cashRegister = await _apiService.GetAsync<CashRegisterDto>($"/cashregister/open/{user.UserId}");

            if (cashRegister != null)
            {
                HasCashRegister = true;
                CashRegisterId = cashRegister.Id;
                OpeningBalance = cashRegister.OpeningBalance;
                OpeningDate = cashRegister.OpeningDate;
            }
            else
            {
                HasCashRegister = false;
                CashRegisterId = null;
            }
        }
        catch
        {
            HasCashRegister = false;
        }
    }

    [RelayCommand]
    async Task OpenCashRegister()
    {
        if (IsBusy) return;

        if (OpeningBalance <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "El balance inicial debe ser mayor a cero", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            var request = new
            {
                UserId = user.UserId,
                OpeningBalance
            };

            var result = await _apiService.PostAsync<CashRegisterDto>("/cashregister/open", request);

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Éxito", "Caja abierta correctamente", "OK");
                await LoadCashRegister();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al abrir caja: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task CloseCashRegister()
    {
        if (IsBusy || !CashRegisterId.HasValue) return;

        if (ClosingBalance < 0)
        {
            await Shell.Current.DisplayAlert("Error", "El balance de cierre debe ser positivo", "OK");
            return;
        }

        var confirm = await Shell.Current.DisplayAlert(
            "Confirmar",
            "¿Está seguro de cerrar la caja?",
            "Sí",
            "No");

        if (!confirm) return;

        try
        {
            IsBusy = true;

            var request = new
            {
                Id = CashRegisterId.Value,
                ClosingBalance
            };

            var result = await _apiService.PostAsync<CashRegisterDto>($"/cashregister/close/{CashRegisterId}", request);

            if (result != null)
            {
                var message = $"Caja cerrada\n" +
                    $"Balance esperado: ${result.ExpectedBalance:N2}\n" +
                    $"Balance real: ${result.ClosingBalance:N2}\n" +
                    $"Diferencia: ${result.Difference:N2}";

                await Shell.Current.DisplayAlert("Caja Cerrada", message, "OK");
                await LoadCashRegister();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al cerrar caja: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public class CashRegisterDto
{
    public Guid Id { get; set; }
    public DateTime OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal? ClosingBalance { get; set; }
    public decimal? ExpectedBalance { get; set; }
    public decimal? Difference { get; set; }
    public string Status { get; set; } = string.Empty;
}
