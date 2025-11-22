using AutoMapper;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.DTOs.Reports;
using CashFlowSystem.Application.Queries.Reports;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Reports;

public class GenerateReportQueryHandler : IRequestHandler<GenerateReportQuery, ReportDto>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IMapper _mapper;

    public GenerateReportQueryHandler(IRepository<Transaction> transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<ReportDto> Handle(GenerateReportQuery request, CancellationToken cancellationToken)
    {
        var allTransactions = await _transactionRepository.GetAllAsync(cancellationToken);

        // Filter transactions
        var filtered = allTransactions
            .Where(t => t.Date >= request.StartDate && t.Date <= request.EndDate);

        if (request.UserId.HasValue)
        {
            filtered = filtered.Where(t => t.UserId == request.UserId.Value);
        }

        if (!string.IsNullOrEmpty(request.Type))
        {
            if (Enum.TryParse<TransactionType>(request.Type, true, out var transactionType))
            {
                filtered = filtered.Where(t => t.Type == transactionType);
            }
        }

        if (request.CategoryId.HasValue)
        {
            filtered = filtered.Where(t => t.CategoryId == request.CategoryId.Value);
        }

        var transactions = filtered.OrderByDescending(t => t.Date).ToList();

        // Calculate summary
        var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        var netFlow = totalIncome - totalExpense;
        var transactionCount = transactions.Count;
        var averageTransaction = transactionCount > 0 ? transactions.Sum(t => t.Amount) / transactionCount : 0;

        // Category breakdown
        var categoryBreakdown = transactions
            .GroupBy(t => t.Category.Name)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        // Determine title
        var title = string.IsNullOrEmpty(request.Type)
            ? "Cash Flow Report"
            : $"{request.Type} Report";

        return new ReportDto
        {
            Title = title,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            GeneratedAt = DateTime.UtcNow,
            Summary = new ReportSummary
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                NetFlow = netFlow,
                TransactionCount = transactionCount,
                AverageTransaction = averageTransaction
            },
            Transactions = _mapper.Map<List<TransactionDto>>(transactions),
            CategoryBreakdown = categoryBreakdown
        };
    }
}
