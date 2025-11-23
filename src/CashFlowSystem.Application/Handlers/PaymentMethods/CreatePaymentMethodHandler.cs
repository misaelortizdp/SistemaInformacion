using CashFlowSystem.Application.Commands.PaymentMethods;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.PaymentMethods;

public class CreatePaymentMethodHandler : IRequestHandler<CreatePaymentMethodCommand, PaymentMethodDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentMethodHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentMethodDto> Handle(CreatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var paymentMethod = new PaymentMethod
        {
            Name = request.Name,
            Description = request.Description,
            Type = request.Type
        };

        await _unitOfWork.PaymentMethods.AddAsync(paymentMethod);
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
