using MediatR;

namespace CashFlowSystem.Application.Commands.Budgets;

public record DeleteBudgetCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}
