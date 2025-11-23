using CashFlowSystem.MAUI.Views;

namespace CashFlowSystem.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes
        Routing.RegisterRoute("addtransaction", typeof(AddTransactionPage));
    }
}
