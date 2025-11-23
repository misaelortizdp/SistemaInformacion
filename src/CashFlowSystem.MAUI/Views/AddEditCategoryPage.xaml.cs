using CashFlowSystem.MAUI.ViewModels;

namespace CashFlowSystem.MAUI.Views;

public partial class AddEditCategoryPage : ContentPage
{
    public AddEditCategoryPage(AddEditCategoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
