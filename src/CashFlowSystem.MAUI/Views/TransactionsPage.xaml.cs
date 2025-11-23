using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class TransactionsPage : ContentPage
{
    public TransactionsPage(TransactionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((TransactionsViewModel)BindingContext).LoadTransactionsCommand.ExecuteAsync(null);
    }
}
