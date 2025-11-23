using CashFlowSystem.Application.Commands.PaymentMethods;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.PaymentMethods;

public class DeletePaymentMethodHandler : IRequestHandler<DeletePaymentMethodCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePaymentMethodHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(request.Id);
        if (paymentMethod == null)
            return false;

        // Check if payment method has transactions
        var transactions = await _unitOfWork.Transactions.GetAllAsync();
        if (transactions.Any(t => t.PaymentMethodId == request.Id && !t.IsDeleted))
        {
            throw new InvalidOperationException("Cannot delete payment method with associated transactions");
        }

        _unitOfWork.PaymentMethods.Delete(paymentMethod);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
