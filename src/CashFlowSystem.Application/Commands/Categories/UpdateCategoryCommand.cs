using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Enums;
using MediatR;

namespace CashFlowSystem.Application.Commands.Categories;

public record UpdateCategoryCommand : IRequest<CategoryDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TransactionType Type { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
}
