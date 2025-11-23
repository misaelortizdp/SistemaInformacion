using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Queries.Budgets;

public record GetUserBudgetsQuery : IRequest<IEnumerable<BudgetDto>>
{
    public Guid UserId { get; init; }
    public bool? IsActive { get; init; }
}
