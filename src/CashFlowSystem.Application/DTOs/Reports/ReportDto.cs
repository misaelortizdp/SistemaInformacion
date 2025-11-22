namespace CashFlowSystem.Application.DTOs.Reports;

public class ReportDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public ReportSummary Summary { get; set; } = new();
    public List<TransactionDto> Transactions { get; set; } = new();
    public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
}

public class ReportSummary
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetFlow { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageTransaction { get; set; }
}
