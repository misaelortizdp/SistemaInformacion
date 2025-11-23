using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
