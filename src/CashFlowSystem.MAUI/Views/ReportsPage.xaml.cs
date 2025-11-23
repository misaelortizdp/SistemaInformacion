using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class ReportsPage : ContentPage
{
    public ReportsPage(ReportsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
