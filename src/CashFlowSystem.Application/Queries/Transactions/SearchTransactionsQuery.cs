using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Queries.Transactions;

public record SearchTransactionsQuery : IRequest<IEnumerable<TransactionDto>>
{
    public Guid? UserId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Type { get; init; }
    public string? SearchText { get; init; } // Busca en descripción, notas, referencia
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public List<Guid>? CategoryIds { get; init; }
    public List<Guid>? PaymentMethodIds { get; init; }
    public string? OrderBy { get; init; } // date, amount, description
    public bool Descending { get; init; } = true;
    public int? Skip { get; init; }
    public int? Take { get; init; }
}
