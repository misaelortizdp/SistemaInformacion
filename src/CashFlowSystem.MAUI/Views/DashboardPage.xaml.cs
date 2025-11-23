using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((DashboardViewModel)BindingContext).LoadDashboardCommand.ExecuteAsync(null);
    }
}
