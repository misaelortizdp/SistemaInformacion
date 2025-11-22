using CashFlowSystem.Application.Commands.CashRegister;
using CashFlowSystem.Application.Queries.CashRegister;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.CashRegister;

public class GetOpenCashRegisterQueryHandler : IRequestHandler<GetOpenCashRegisterQuery, CashRegisterDto?>
{
    private readonly IRepository<Domain.Entities.CashRegister> _cashRegisterRepository;

    public GetOpenCashRegisterQueryHandler(IRepository<Domain.Entities.CashRegister> cashRegisterRepository)
    {
        _cashRegisterRepository = cashRegisterRepository;
    }

    public async Task<CashRegisterDto?> Handle(GetOpenCashRegisterQuery request, CancellationToken cancellationToken)
    {
        var cashRegisters = await _cashRegisterRepository.GetAllAsync(cancellationToken);
        var openRegister = cashRegisters.FirstOrDefault(cr =>
            cr.UserId == request.UserId && cr.Status == CashRegisterStatus.Open);

        if (openRegister == null)
        {
            return null;
        }

        return new CashRegisterDto
        {
            Id = openRegister.Id,
            OpeningDate = openRegister.OpeningDate,
            ClosingDate = openRegister.ClosingDate,
            OpeningBalance = openRegister.OpeningBalance,
            ClosingBalance = openRegister.ClosingBalance,
            ExpectedBalance = openRegister.ExpectedBalance,
            Difference = openRegister.Difference,
            Status = openRegister.Status.ToString(),
            Notes = openRegister.Notes,
            UserId = openRegister.UserId
        };
    }
}
