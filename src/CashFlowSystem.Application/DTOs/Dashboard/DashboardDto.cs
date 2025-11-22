namespace CashFlowSystem.Application.DTOs.Dashboard;

public class DashboardDto
{
    public decimal CurrentBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetFlow { get; set; }
    public List<TransactionDto> RecentTransactions { get; set; } = new();
    public Dictionary<string, decimal> IncomeByCategory { get; set; } = new();
    public Dictionary<string, decimal> ExpenseByCategory { get; set; } = new();
    public List<DailyFlowDto> DailyFlow { get; set; } = new();
}

public class DailyFlowDto
{
    public DateTime Date { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Balance { get; set; }
}
