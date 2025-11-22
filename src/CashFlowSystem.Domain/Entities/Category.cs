using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public string Color { get; set; } = "#000000";
    public string Icon { get; set; } = "default";
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
