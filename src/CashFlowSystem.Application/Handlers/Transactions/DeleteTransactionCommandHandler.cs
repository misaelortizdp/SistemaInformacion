using CashFlowSystem.Application.Commands.Transactions;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Transactions;

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, bool>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTransactionCommandHandler(
        IRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (transaction == null)
        {
            return false;
        }

        await _transactionRepository.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
