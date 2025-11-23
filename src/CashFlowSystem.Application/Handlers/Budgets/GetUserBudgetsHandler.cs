using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.Queries.Budgets;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Budgets;

public class GetUserBudgetsHandler : IRequestHandler<GetUserBudgetsQuery, IEnumerable<BudgetDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserBudgetsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BudgetDto>> Handle(GetUserBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgets = await _unitOfWork.Budgets.GetAllAsync();
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var transactions = await _unitOfWork.Transactions.GetAllAsync();

        var query = budgets.Where(b => !b.IsDeleted && b.UserId == request.UserId);

        if (request.IsActive.HasValue)
        {
            query = query.Where(b => b.IsActive == request.IsActive.Value);
        }

        var result = query.Select(budget =>
        {
            var spentAmount = transactions
                .Where(t => !t.IsDeleted &&
                           t.CategoryId == budget.CategoryId &&
                           t.UserId == budget.UserId &&
                           t.Date >= budget.StartDate &&
                           t.Date <= budget.EndDate &&
                           t.Type == Domain.Enums.TransactionType.Expense)
                .Sum(t => t.Amount);

            var category = categories.FirstOrDefault(c => c.Id == budget.CategoryId);

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
        }).ToList();

        return result;
    }
}
