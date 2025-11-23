using CashFlowSystem.Application.Commands.PaymentMethods;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.PaymentMethods;

public class UpdatePaymentMethodHandler : IRequestHandler<UpdatePaymentMethodCommand, PaymentMethodDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentMethodHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentMethodDto> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(request.Id);
        if (paymentMethod == null)
            throw new KeyNotFoundException($"Payment method with ID {request.Id} not found");

        paymentMethod.Name = request.Name;
        paymentMethod.Description = request.Description;
        paymentMethod.Type = request.Type;

        _unitOfWork.PaymentMethods.Update(paymentMethod);
        await _unitOfWork.SaveChangesAsync();

        return new PaymentMethodDto
        {
            Id = paymentMethod.Id,
            Name = paymentMethod.Name,
            Description = paymentMethod.Description,
            Type = paymentMethod.Type.ToString()
        };
    }
}
