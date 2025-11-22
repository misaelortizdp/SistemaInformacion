using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Commands.Transactions;

public class UpdateTransactionCommand : IRequest<TransactionDto>
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
