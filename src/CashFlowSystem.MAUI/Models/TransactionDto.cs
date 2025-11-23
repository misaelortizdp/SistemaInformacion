namespace CashFlowSystem.MAUI.Models;

public class TransactionDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid PaymentMethodId { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}

public class CreateTransactionRequest
{
    public DateTime Date { get; set; } = DateTime.Now;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "Income";
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid UserId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
