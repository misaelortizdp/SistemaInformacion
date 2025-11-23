using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class AddTransactionViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private DateTime date = DateTime.Now;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string type = "Income";

    [ObservableProperty]
    private CategoryDto? selectedCategory;

    [ObservableProperty]
    private PaymentMethodDto? selectedPaymentMethod;

    [ObservableProperty]
    private string? referenceNumber;

    [ObservableProperty]
    private string? notes;

    [ObservableProperty]
    private bool isBusy;

    public ObservableCollection<CategoryDto> Categories { get; } = new();
    public ObservableCollection<PaymentMethodDto> PaymentMethods { get; } = new();
    public ObservableCollection<string> TransactionTypes { get; } = new() { "Income", "Expense" };

    public AddTransactionViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
    }

    [RelayCommand]
    async Task LoadData()
    {
        try
        {
            var categories = await _apiService.GetAsync<List<CategoryDto>>($"/categories?type={Type}");
            var paymentMethods = await _apiService.GetAsync<List<PaymentMethodDto>>("/paymentmethods");

            Categories.Clear();
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }

            PaymentMethods.Clear();
            if (paymentMethods != null)
            {
                foreach (var method in paymentMethods)
                {
                    PaymentMethods.Add(method);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    async Task SaveTransaction()
    {
        if (IsBusy) return;

        if (Amount <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "El monto debe ser mayor a cero", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            await Shell.Current.DisplayAlert("Error", "Ingrese una descripción", "OK");
            return;
        }

        if (SelectedCategory == null || SelectedPaymentMethod == null)
        {
            await Shell.Current.DisplayAlert("Error", "Seleccione categoría y método de pago", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                await Shell.Current.DisplayAlert("Error", "Usuario no autenticado", "OK");
                return;
            }

            var request = new CreateTransactionRequest
            {
                Date = Date,
                Amount = Amount,
                Description = Description,
                Type = Type,
                CategoryId = SelectedCategory.Id,
                PaymentMethodId = SelectedPaymentMethod.Id,
                UserId = user.UserId,
                ReferenceNumber = ReferenceNumber,
                Notes = Notes
            };

            var result = await _apiService.PostAsync<TransactionDto>("/transactions", request);

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Éxito", "Transacción guardada", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo guardar la transacción", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
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

    partial void OnTypeChanged(string value)
    {
        _ = LoadData();
    }
}
