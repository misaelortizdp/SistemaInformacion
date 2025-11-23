using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Enums;
using MediatR;

namespace CashFlowSystem.Application.Commands.Categories;

public record CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TransactionType Type { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
}
