using AutoMapper;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.Queries.Transactions;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Transactions;

public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, IEnumerable<TransactionDto>>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IMapper _mapper;

    public GetAllTransactionsQueryHandler(
        IRepository<Transaction> transactionRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken);

        // Apply filters
        var filtered = transactions.AsEnumerable();

        if (request.UserId.HasValue)
        {
            filtered = filtered.Where(t => t.UserId == request.UserId.Value);
        }

        if (request.StartDate.HasValue)
        {
            filtered = filtered.Where(t => t.Date >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            filtered = filtered.Where(t => t.Date <= request.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(request.Type))
        {
            if (Enum.TryParse<TransactionType>(request.Type, true, out var transactionType))
            {
                filtered = filtered.Where(t => t.Type == transactionType);
            }
        }

        // Order by date descending
        var ordered = filtered.OrderByDescending(t => t.Date).ToList();

        return _mapper.Map<IEnumerable<TransactionDto>>(ordered);
    }
}
