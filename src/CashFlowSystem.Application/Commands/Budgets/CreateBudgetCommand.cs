using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Commands.Budgets;

public record CreateBudgetCommand : IRequest<BudgetDto>
{
    public Guid UserId { get; init; }
    public Guid CategoryId { get; init; }
    public decimal Amount { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string? Description { get; init; }
}
