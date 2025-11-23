using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class CashRegisterPage : ContentPage
{
    public CashRegisterPage(CashRegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((CashRegisterViewModel)BindingContext).LoadCashRegisterCommand.ExecuteAsync(null);
    }
}
