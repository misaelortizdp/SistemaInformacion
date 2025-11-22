using AutoMapper;
using CashFlowSystem.Application.Commands.Transactions;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Transactions;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<PaymentMethod> _paymentMethodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(
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

    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
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

        // Parse transaction type
        if (!Enum.TryParse<TransactionType>(request.Type, true, out var transactionType))
        {
            throw new ArgumentException($"Invalid transaction type: {request.Type}");
        }

        // Create transaction
        var transaction = new Transaction
        {
            Date = request.Date,
            Amount = request.Amount,
            Description = request.Description,
            Type = transactionType,
            CategoryId = request.CategoryId,
            PaymentMethodId = request.PaymentMethodId,
            UserId = request.UserId,
            ReferenceNumber = request.ReferenceNumber,
            Notes = request.Notes
        };

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with navigation properties
        var createdTransaction = await _transactionRepository.GetByIdAsync(transaction.Id, cancellationToken);
        return _mapper.Map<TransactionDto>(createdTransaction);
    }
}
