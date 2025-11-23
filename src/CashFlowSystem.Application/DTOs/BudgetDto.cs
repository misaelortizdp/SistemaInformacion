namespace CashFlowSystem.Application.DTOs;

public class BudgetDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public decimal SpentAmount { get; set; } // Calculated
    public decimal RemainingAmount { get; set; } // Calculated
    public decimal PercentageUsed { get; set; } // Calculated
}
