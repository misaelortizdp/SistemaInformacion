using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class SearchTransactionsPage : ContentPage
{
    public SearchTransactionsPage(SearchTransactionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((SearchTransactionsViewModel)BindingContext).LoadDataCommand.ExecuteAsync(null);
    }
}
