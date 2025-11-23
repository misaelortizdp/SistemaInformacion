using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class AddEditPaymentMethodPage : ContentPage
{
    public AddEditPaymentMethodPage(AddEditPaymentMethodViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
