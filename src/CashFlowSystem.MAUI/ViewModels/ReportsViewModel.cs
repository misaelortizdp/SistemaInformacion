using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private DateTime startDate = DateTime.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTime endDate = DateTime.Now;

    [ObservableProperty]
    private string title = "Informe de Flujo de Caja";

    [ObservableProperty]
    private decimal totalIncome;

    [ObservableProperty]
    private decimal totalExpense;

    [ObservableProperty]
    private decimal netFlow;

    [ObservableProperty]
    private int transactionCount;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool hasData;

    public ReportsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    async Task GenerateReport()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            HasData = false;

            var endpoint = $"/reports?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}";
            var report = await _apiService.GetAsync<ReportDto>(endpoint);

            if (report != null)
            {
                Title = report.Title;
                TotalIncome = report.Summary.TotalIncome;
                TotalExpense = report.Summary.TotalExpense;
                NetFlow = report.Summary.NetFlow;
                TransactionCount = report.Summary.TransactionCount;
                HasData = true;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo generar el informe", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al generar informe: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public class ReportDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReportSummary Summary { get; set; } = new();
}

public class ReportSummary
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetFlow { get; set; }
    public int TransactionCount { get; set; }
}
