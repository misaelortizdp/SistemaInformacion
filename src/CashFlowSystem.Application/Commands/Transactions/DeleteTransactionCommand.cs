using MediatR;

namespace CashFlowSystem.Application.Commands.Transactions;

public class DeleteTransactionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
