using CashFlowSystem.Application.Commands.Budgets;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Budgets;

public class CreateBudgetHandler : IRequestHandler<CreateBudgetCommand, BudgetDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBudgetHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BudgetDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        // Validate date range
        if (request.EndDate <= request.StartDate)
            throw new ArgumentException("End date must be after start date");

        // Validate amount
        if (request.Amount <= 0)
            throw new ArgumentException("Budget amount must be greater than zero");

        var budget = new Budget
        {
            UserId = request.UserId,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description,
            IsActive = true
        };

        await _unitOfWork.Budgets.AddAsync(budget);
        await _unitOfWork.SaveChangesAsync();

        // Calculate spent amount
        var transactions = await _unitOfWork.Transactions.GetAllAsync();
        var spentAmount = transactions
            .Where(t => !t.IsDeleted &&
                       t.CategoryId == budget.CategoryId &&
                       t.UserId == budget.UserId &&
                       t.Date >= budget.StartDate &&
                       t.Date <= budget.EndDate &&
                       t.Type == Domain.Enums.TransactionType.Expense)
            .Sum(t => t.Amount);

        var category = await _unitOfWork.Categories.GetByIdAsync(budget.CategoryId);

        return new BudgetDto
        {
            Id = budget.Id,
            UserId = budget.UserId,
            CategoryId = budget.CategoryId,
            CategoryName = category?.Name ?? "",
            Amount = budget.Amount,
            StartDate = budget.StartDate,
            EndDate = budget.EndDate,
            Description = budget.Description,
            IsActive = budget.IsActive,
            SpentAmount = spentAmount,
            RemainingAmount = budget.Amount - spentAmount,
            PercentageUsed = budget.Amount > 0 ? (spentAmount / budget.Amount) * 100 : 0
        };
    }
}
