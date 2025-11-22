using AutoMapper;
using CashFlowSystem.Application.Commands.Transactions;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Transactions;

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, TransactionDto>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<PaymentMethod> _paymentMethodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateTransactionCommandHandler(
        IRepository<Transaction> transactionRepository,
        IRepository<Category> categoryRepository,
        IRepository<PaymentMethod> paymentMethodRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TransactionDto> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (transaction == null)
        {
            throw new InvalidOperationException($"Transaction with ID {request.Id} not found");
        }

        // Validate category exists
        if (!await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken))
        {
            throw new InvalidOperationException($"Category with ID {request.CategoryId} not found");
        }

        // Validate payment method exists
        if (!await _paymentMethodRepository.ExistsAsync(request.PaymentMethodId, cancellationToken))
        {
            throw new InvalidOperationException($"Payment method with ID {request.PaymentMethodId} not found");
        }

        // Update transaction
        transaction.Date = request.Date;
        transaction.Amount = request.Amount;
        transaction.Description = request.Description;
        transaction.CategoryId = request.CategoryId;
        transaction.PaymentMethodId = request.PaymentMethodId;
        transaction.ReferenceNumber = request.ReferenceNumber;
        transaction.Notes = request.Notes;

        await _transactionRepository.UpdateAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with navigation properties
        var updatedTransaction = await _transactionRepository.GetByIdAsync(transaction.Id, cancellationToken);
        return _mapper.Map<TransactionDto>(updatedTransaction);
    }
}
