using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class TransactionsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string filterType = "Todas";

    public ObservableCollection<TransactionDto> Transactions { get; } = new();
    public ObservableCollection<string> FilterOptions { get; } = new() { "Todas", "Ingresos", "Egresos" };

    public TransactionsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    async Task LoadTransactions()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var typeFilter = FilterType switch
            {
                "Ingresos" => "Income",
                "Egresos" => "Expense",
                _ => null
            };

            var endpoint = string.IsNullOrEmpty(typeFilter)
                ? "/transactions"
                : $"/transactions?type={typeFilter}";

            var transactions = await _apiService.GetAsync<List<TransactionDto>>(endpoint);

            if (transactions != null)
            {
                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al cargar transacciones: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddTransaction()
    {
        await Shell.Current.GoToAsync("addtransaction");
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
                Transactions.Remove(transaction);
                await Shell.Current.DisplayAlert("Éxito", "Transacción eliminada", "OK");
            }
        }
    }

    partial void OnFilterTypeChanged(string value)
    {
        _ = LoadTransactions();
    }
}
