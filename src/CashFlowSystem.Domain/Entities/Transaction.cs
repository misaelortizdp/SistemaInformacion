using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Domain.Entities;

public class Transaction : BaseEntity
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }

    // Foreign Keys
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid UserId { get; set; }
    public Guid? CashRegisterId { get; set; }

    // Navigation properties
    public Category Category { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public User User { get; set; } = null!;
    public CashRegister? CashRegister { get; set; }

    // Additional fields
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
