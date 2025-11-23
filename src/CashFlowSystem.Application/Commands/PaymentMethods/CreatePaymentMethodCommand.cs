using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Enums;
using MediatR;

namespace CashFlowSystem.Application.Commands.PaymentMethods;

public record CreatePaymentMethodCommand : IRequest<PaymentMethodDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public PaymentMethodType Type { get; init; }
}
