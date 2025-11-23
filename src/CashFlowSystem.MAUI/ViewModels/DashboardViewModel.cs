using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private decimal currentBalance;

    [ObservableProperty]
    private decimal totalIncome;

    [ObservableProperty]
    private decimal totalExpense;

    [ObservableProperty]
    private decimal netFlow;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string userName = string.Empty;

    public ObservableCollection<TransactionDto> RecentTransactions { get; } = new();

    public DashboardViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
    }

    [RelayCommand]
    async Task LoadDashboard()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = await _authService.GetCurrentUserAsync();
            if (user != null)
            {
                UserName = user.Username;
            }

            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = DateTime.Now;

            var dashboard = await _apiService.GetAsync<DashboardDto>(
                $"/dashboard?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (dashboard != null)
            {
                CurrentBalance = dashboard.CurrentBalance;
                TotalIncome = dashboard.TotalIncome;
                TotalExpense = dashboard.TotalExpense;
                NetFlow = dashboard.NetFlow;

                RecentTransactions.Clear();
                foreach (var transaction in dashboard.RecentTransactions)
                {
                    RecentTransactions.Add(transaction);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al cargar dashboard: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task Logout()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//login");
    }
}
