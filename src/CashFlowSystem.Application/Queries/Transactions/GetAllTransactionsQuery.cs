using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Queries.Transactions;

public class GetAllTransactionsQuery : IRequest<IEnumerable<TransactionDto>>
{
    public Guid? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Type { get; set; } // "Income", "Expense", or null for all
}
