using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class PaymentMethodsPage : ContentPage
{
    public PaymentMethodsPage(PaymentMethodsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((PaymentMethodsViewModel)BindingContext).LoadPaymentMethodsCommand.ExecuteAsync(null);
    }
}
