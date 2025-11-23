using MediatR;

namespace CashFlowSystem.Application.Commands.Categories;

public record DeleteCategoryCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}
