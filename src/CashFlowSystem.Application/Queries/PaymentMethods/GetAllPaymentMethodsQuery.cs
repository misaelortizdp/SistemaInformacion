using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Queries.PaymentMethods;

public class GetAllPaymentMethodsQuery : IRequest<IEnumerable<PaymentMethodDto>>
{
}
