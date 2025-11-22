using AutoMapper;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.DTOs.Dashboard;
using CashFlowSystem.Application.Queries.Dashboard;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Dashboard;

public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDto>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IMapper _mapper;

    public GetDashboardDataQueryHandler(IRepository<Transaction> transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<DashboardDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        var allTransactions = await _transactionRepository.GetAllAsync(cancellationToken);

        // Filter by date range and user
        var filtered = allTransactions
            .Where(t => t.Date >= request.StartDate && t.Date <= request.EndDate);

        if (request.UserId.HasValue)
        {
            filtered = filtered.Where(t => t.UserId == request.UserId.Value);
        }

        var transactions = filtered.ToList();

        // Calculate totals
        var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        var netFlow = totalIncome - totalExpense;

        // Get recent transactions (last 10)
        var recentTransactions = transactions
            .OrderByDescending(t => t.Date)
            .Take(10)
            .ToList();

        // Income by category
        var incomeByCategory = transactions
            .Where(t => t.Type == TransactionType.Income)
            .GroupBy(t => t.Category.Name)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        // Expense by category
        var expenseByCategory = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.Category.Name)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        // Daily flow
        var dailyFlow = transactions
            .GroupBy(t => t.Date.Date)
            .Select(g => new DailyFlowDto
            {
                Date = g.Key,
                Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                Balance = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) -
                         g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .OrderBy(d => d.Date)
            .ToList();

        return new DashboardDto
        {
            CurrentBalance = netFlow,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            NetFlow = netFlow,
            RecentTransactions = _mapper.Map<List<TransactionDto>>(recentTransactions),
            IncomeByCategory = incomeByCategory,
            ExpenseByCategory = expenseByCategory,
            DailyFlow = dailyFlow
        };
    }
}
