using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Domain.Entities;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public PaymentMethodType Type { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
