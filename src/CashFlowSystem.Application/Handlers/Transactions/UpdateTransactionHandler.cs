using CashFlowSystem.Application.Commands.Transactions;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Transactions;

public class UpdateTransactionHandler : IRequestHandler<UpdateTransactionCommand, TransactionDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTransactionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TransactionDto> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(request.Id);
        if (transaction == null)
            throw new KeyNotFoundException($"Transaction with ID {request.Id} not found");

        // Check if transaction belongs to a closed cash register
        if (transaction.CashRegisterId.HasValue)
        {
            var cashRegister = await _unitOfWork.CashRegisters.GetByIdAsync(transaction.CashRegisterId.Value);
            if (cashRegister != null && cashRegister.Status == CashRegisterStatus.Closed)
            {
                throw new InvalidOperationException("Cannot edit transactions from a closed cash register");
            }
        }

        // Update transaction properties
        transaction.Amount = request.Amount;
        transaction.Description = request.Description;
        transaction.Date = request.Date;
        transaction.Type = request.Type;
        transaction.CategoryId = request.CategoryId;
        transaction.PaymentMethodId = request.PaymentMethodId;
        transaction.ReferenceNumber = request.ReferenceNumber;
        transaction.Notes = request.Notes;

        _unitOfWork.Transactions.Update(transaction);
        await _unitOfWork.SaveChangesAsync();

        // Get related entities for DTO
        var category = await _unitOfWork.Categories.GetByIdAsync(transaction.CategoryId);
        var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(transaction.PaymentMethodId);

        return new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Date = transaction.Date,
            Type = transaction.Type.ToString(),
            CategoryId = transaction.CategoryId,
            CategoryName = category?.Name ?? "",
            PaymentMethodId = transaction.PaymentMethodId,
            PaymentMethodName = paymentMethod?.Name ?? "",
            ReferenceNumber = transaction.ReferenceNumber,
            Notes = transaction.Notes
        };
    }
}
