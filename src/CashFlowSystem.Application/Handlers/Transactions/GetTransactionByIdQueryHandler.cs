using AutoMapper;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.Queries.Transactions;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Transactions;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionByIdQueryHandler(
        IRepository<Transaction> transactionRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<TransactionDto> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (transaction == null)
        {
            throw new InvalidOperationException($"Transaction with ID {request.Id} not found");
        }

        return _mapper.Map<TransactionDto>(transaction);
    }
}
