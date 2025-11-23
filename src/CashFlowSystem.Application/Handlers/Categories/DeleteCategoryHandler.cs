using CashFlowSystem.Application.Commands.Categories;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Categories;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
        if (category == null)
            return false;

        // Check if category has transactions
        var transactions = await _unitOfWork.Transactions.GetAllAsync();
        if (transactions.Any(t => t.CategoryId == request.Id && !t.IsDeleted))
        {
            throw new InvalidOperationException("Cannot delete category with associated transactions");
        }

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
