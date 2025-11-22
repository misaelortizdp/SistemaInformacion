using CashFlowSystem.Application.Commands.CashRegister;
using MediatR;

namespace CashFlowSystem.Application.Queries.CashRegister;

public class GetOpenCashRegisterQuery : IRequest<CashRegisterDto?>
{
    public Guid UserId { get; set; }
}
