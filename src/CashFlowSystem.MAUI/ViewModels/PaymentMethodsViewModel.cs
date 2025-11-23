using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class PaymentMethodsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string filterType = "All";

    public ObservableCollection<PaymentMethodDto> PaymentMethods { get; } = new();

    public List<string> FilterOptions { get; } = new() { "All", "Cash", "BankTransfer", "Card", "Other" };

    public PaymentMethodsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    async Task LoadPaymentMethods()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            PaymentMethods.Clear();

            var endpoint = FilterType == "All"
                ? "/paymentmethods"
                : $"/paymentmethods?type={FilterType}";

            var paymentMethods = await _apiService.GetAsync<List<PaymentMethodDto>>(endpoint);

            if (paymentMethods != null)
            {
                foreach (var pm in paymentMethods)
                {
                    PaymentMethods.Add(pm);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los métodos de pago: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddPaymentMethod()
    {
        await Shell.Current.GoToAsync("AddEditPaymentMethod");
    }

    [RelayCommand]
    async Task EditPaymentMethod(PaymentMethodDto paymentMethod)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "PaymentMethod", paymentMethod }
        };

        await Shell.Current.GoToAsync("AddEditPaymentMethod", navigationParameter);
    }

    [RelayCommand]
    async Task DeletePaymentMethod(PaymentMethodDto paymentMethod)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Confirmar eliminación",
            $"¿Está seguro de eliminar el método de pago '{paymentMethod.Name}'?",
            "Eliminar",
            "Cancelar");

        if (!confirm) return;

        try
        {
            IsBusy = true;

            var success = await _apiService.DeleteAsync($"/paymentmethods/{paymentMethod.Id}");

            if (success)
            {
                PaymentMethods.Remove(paymentMethod);
                await Shell.Current.DisplayAlert("Éxito", "Método de pago eliminado", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo eliminar el método de pago", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al eliminar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnFilterTypeChanged(string value)
    {
        _ = LoadPaymentMethods();
    }
}
