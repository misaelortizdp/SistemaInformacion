using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Commands.Transactions;

public class CreateTransactionCommand : IRequest<TransactionDto>
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Income" or "Expense"
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid UserId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
