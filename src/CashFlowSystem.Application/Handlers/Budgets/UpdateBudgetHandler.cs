using CashFlowSystem.Application.Commands.Budgets;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Budgets;

public class UpdateBudgetHandler : IRequestHandler<UpdateBudgetCommand, BudgetDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBudgetHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BudgetDto> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _unitOfWork.Budgets.GetByIdAsync(request.Id);
        if (budget == null)
            throw new KeyNotFoundException($"Budget with ID {request.Id} not found");

        if (request.EndDate <= request.StartDate)
            throw new ArgumentException("End date must be after start date");

        if (request.Amount <= 0)
            throw new ArgumentException("Budget amount must be greater than zero");

        budget.Amount = request.Amount;
        budget.StartDate = request.StartDate;
        budget.EndDate = request.EndDate;
        budget.Description = request.Description;
        budget.IsActive = request.IsActive;

        _unitOfWork.Budgets.Update(budget);
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
