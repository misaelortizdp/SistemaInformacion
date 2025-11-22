using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Domain.Entities;

public class CashRegister : BaseEntity
{
    public DateTime OpeningDate { get; set; } = DateTime.UtcNow;
    public DateTime? ClosingDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal? ClosingBalance { get; set; }
    public decimal? ExpectedBalance { get; set; }
    public decimal? Difference { get; set; }
    public CashRegisterStatus Status { get; set; } = CashRegisterStatus.Open;
    public string? Notes { get; set; }

    // Foreign Keys
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
