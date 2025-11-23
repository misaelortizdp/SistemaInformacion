using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Enums;
using MediatR;

namespace CashFlowSystem.Application.Commands.PaymentMethods;

public record UpdatePaymentMethodCommand : IRequest<PaymentMethodDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public PaymentMethodType Type { get; init; }
}
