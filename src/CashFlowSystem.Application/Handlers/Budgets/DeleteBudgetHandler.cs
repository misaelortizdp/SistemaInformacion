using CashFlowSystem.Application.Commands.Budgets;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Budgets;

public class DeleteBudgetHandler : IRequestHandler<DeleteBudgetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBudgetHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _unitOfWork.Budgets.GetByIdAsync(request.Id);
        if (budget == null)
            return false;

        _unitOfWork.Budgets.Delete(budget);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
