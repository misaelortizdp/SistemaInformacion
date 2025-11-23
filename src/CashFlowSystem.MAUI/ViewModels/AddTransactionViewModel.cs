using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashFlowSystem.MAUI.ViewModels;

[QueryProperty(nameof(Transaction), "Transaction")]
public partial class AddTransactionViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private TransactionDto? transaction;

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

    [ObservableProperty]
    private bool isEditMode;

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

            TransactionDto? result;

            if (IsEditMode && Transaction != null)
            {
                result = await _apiService.PutAsync<TransactionDto>($"/transactions/{Transaction.Id}", request);
            }
            else
            {
                result = await _apiService.PostAsync<TransactionDto>("/transactions", request);
            }

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Éxito",
                    IsEditMode ? "Transacción actualizada" : "Transacción creada", "OK");
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

    async partial void OnTransactionChanged(TransactionDto? value)
    {
        if (value != null)
        {
            IsEditMode = true;
            Date = value.Date;
            Amount = value.Amount;
            Description = value.Description;
            Type = value.Type;
            ReferenceNumber = value.ReferenceNumber;
            Notes = value.Notes;

            // Load data first to populate categories and payment methods
            await LoadData();

            // Then set the selected items
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == value.CategoryId);
            SelectedPaymentMethod = PaymentMethods.FirstOrDefault(p => p.Id == value.PaymentMethodId);
        }
        else
        {
            IsEditMode = false;
        }
    }
}
