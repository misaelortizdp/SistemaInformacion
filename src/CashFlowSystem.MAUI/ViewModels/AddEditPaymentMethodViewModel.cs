using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CashFlowSystem.MAUI.ViewModels;

[QueryProperty(nameof(PaymentMethod), "PaymentMethod")]
public partial class AddEditPaymentMethodViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private PaymentMethodDto? paymentMethod;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string type = "Cash";

    [ObservableProperty]
    private string icon = "💵";

    [ObservableProperty]
    private string color = "#4CAF50";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isEditMode;

    public List<string> TypeOptions { get; } = new() { "Cash", "BankTransfer", "Card", "Other" };

    public List<string> IconOptions { get; } = new()
    {
        "💵", "💳", "🏦", "💰", "📱", "💸", "🪙", "🏧",
        "💴", "💶", "💷", "💲", "🤑", "💹", "📲", "🔐"
    };

    public List<string> ColorOptions { get; } = new()
    {
        "#4CAF50", "#2196F3", "#FF9800", "#9C27B0",
        "#F44336", "#00BCD4", "#FFEB3B", "#607D8B"
    };

    public AddEditPaymentMethodViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    partial void OnPaymentMethodChanged(PaymentMethodDto? value)
    {
        if (value != null)
        {
            IsEditMode = true;
            Name = value.Name;
            Description = value.Description ?? string.Empty;
            Type = value.Type;
            Icon = value.Icon ?? "💵";
            Color = value.Color ?? "#4CAF50";
        }
        else
        {
            IsEditMode = false;
        }
    }

    [RelayCommand]
    async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Error", "El nombre es requerido", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var request = new
            {
                Id = PaymentMethod?.Id ?? Guid.Empty,
                Name,
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
                Type,
                Icon,
                Color
            };

            PaymentMethodDto? result;

            if (IsEditMode && PaymentMethod != null)
            {
                result = await _apiService.PutAsync<PaymentMethodDto>($"/paymentmethods/{PaymentMethod.Id}", request);
            }
            else
            {
                result = await _apiService.PostAsync<PaymentMethodDto>("/paymentmethods", request);
            }

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Éxito",
                    IsEditMode ? "Método de pago actualizado" : "Método de pago creado", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
