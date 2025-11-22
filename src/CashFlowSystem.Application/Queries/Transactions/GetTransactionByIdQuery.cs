using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Queries.Transactions;

public class GetTransactionByIdQuery : IRequest<TransactionDto>
{
    public Guid Id { get; set; }
}
