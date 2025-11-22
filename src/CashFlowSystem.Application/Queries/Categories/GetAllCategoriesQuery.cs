using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Queries.Categories;

public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
{
    public string? Type { get; set; } // "Income", "Expense", or null for all
}
