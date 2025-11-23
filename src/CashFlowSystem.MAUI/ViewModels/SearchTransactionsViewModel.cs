using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class SearchTransactionsViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? searchText;

    [ObservableProperty]
    private DateTime? startDate;

    [ObservableProperty]
    private DateTime? endDate;

    [ObservableProperty]
    private string? type;

    [ObservableProperty]
    private decimal? minAmount;

    [ObservableProperty]
    private decimal? maxAmount;

    [ObservableProperty]
    private string orderBy = "date";

    [ObservableProperty]
    private bool descending = true;

    public ObservableCollection<TransactionDto> SearchResults { get; } = new();
    public ObservableCollection<CategoryDto> AvailableCategories { get; } = new();
    public ObservableCollection<PaymentMethodDto> AvailablePaymentMethods { get; } = new();
    public ObservableCollection<CategoryDto> SelectedCategories { get; } = new();
    public ObservableCollection<PaymentMethodDto> SelectedPaymentMethods { get; } = new();

    public List<string> TypeOptions { get; } = new() { "Todos", "Income", "Expense" };
    public List<string> OrderByOptions { get; } = new() { "date", "amount", "description" };

    public SearchTransactionsViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
    }

    [RelayCommand]
    async Task LoadData()
    {
        try
        {
            var categories = await _apiService.GetAsync<List<CategoryDto>>("/categories");
            var paymentMethods = await _apiService.GetAsync<List<PaymentMethodDto>>("/paymentmethods");

            AvailableCategories.Clear();
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    AvailableCategories.Add(category);
                }
            }

            AvailablePaymentMethods.Clear();
            if (paymentMethods != null)
            {
                foreach (var method in paymentMethods)
                {
                    AvailablePaymentMethods.Add(method);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    async Task Search()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            SearchResults.Clear();

            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                await Shell.Current.DisplayAlert("Error", "Usuario no autenticado", "OK");
                return;
            }

            var searchRequest = new
            {
                UserId = user.UserId,
                SearchText = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                StartDate = StartDate,
                EndDate = EndDate,
                Type = Type == "Todos" ? null : Type,
                MinAmount = MinAmount,
                MaxAmount = MaxAmount,
                CategoryIds = SelectedCategories.Count > 0 ? SelectedCategories.Select(c => c.Id).ToList() : null,
                PaymentMethodIds = SelectedPaymentMethods.Count > 0 ? SelectedPaymentMethods.Select(p => p.Id).ToList() : null,
                OrderBy = OrderBy,
                Descending = Descending
            };

            var results = await _apiService.PostAsync<List<TransactionDto>>("/transactions/search", searchRequest);

            if (results != null)
            {
                foreach (var transaction in results)
                {
                    SearchResults.Add(transaction);
                }

                if (results.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Búsqueda", "No se encontraron transacciones con los criterios especificados", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error en la búsqueda: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void ClearFilters()
    {
        SearchText = null;
        StartDate = null;
        EndDate = null;
        Type = "Todos";
        MinAmount = null;
        MaxAmount = null;
        SelectedCategories.Clear();
        SelectedPaymentMethods.Clear();
        OrderBy = "date";
        Descending = true;
        SearchResults.Clear();
    }

    [RelayCommand]
    async Task EditTransaction(TransactionDto transaction)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Transaction", transaction }
        };

        await Shell.Current.GoToAsync("addtransaction", navigationParameter);
    }

    [RelayCommand]
    async Task DeleteTransaction(TransactionDto transaction)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Confirmar",
            $"¿Eliminar transacción '{transaction.Description}'?",
            "Sí",
            "No");

        if (confirm)
        {
            var success = await _apiService.DeleteAsync($"/transactions/{transaction.Id}");
            if (success)
            {
                SearchResults.Remove(transaction);
                await Shell.Current.DisplayAlert("Éxito", "Transacción eliminada", "OK");
            }
        }
    }

    [RelayCommand]
    void ToggleCategory(CategoryDto category)
    {
        if (SelectedCategories.Contains(category))
        {
            SelectedCategories.Remove(category);
        }
        else
        {
            SelectedCategories.Add(category);
        }
    }

    [RelayCommand]
    void TogglePaymentMethod(PaymentMethodDto paymentMethod)
    {
        if (SelectedPaymentMethods.Contains(paymentMethod))
        {
            SelectedPaymentMethods.Remove(paymentMethod);
        }
        else
        {
            SelectedPaymentMethods.Add(paymentMethod);
        }
    }
}
