using MediatR;

namespace CashFlowSystem.Application.Commands.PaymentMethods;

public record DeletePaymentMethodCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}
