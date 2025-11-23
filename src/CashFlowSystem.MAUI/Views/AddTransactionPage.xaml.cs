using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class AddTransactionPage : ContentPage
{
    public AddTransactionPage(AddTransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((AddTransactionViewModel)BindingContext).LoadDataCommand.ExecuteAsync(null);
    }
}
