using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Commands.Budgets;

public record UpdateBudgetCommand : IRequest<BudgetDto>
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}
