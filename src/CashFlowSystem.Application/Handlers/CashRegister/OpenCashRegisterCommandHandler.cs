using CashFlowSystem.Application.Commands.CashRegister;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.CashRegister;

public class OpenCashRegisterCommandHandler : IRequestHandler<OpenCashRegisterCommand, CashRegisterDto>
{
    private readonly IRepository<Domain.Entities.CashRegister> _cashRegisterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OpenCashRegisterCommandHandler(
        IRepository<Domain.Entities.CashRegister> cashRegisterRepository,
        IUnitOfWork unitOfWork)
    {
        _cashRegisterRepository = cashRegisterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CashRegisterDto> Handle(OpenCashRegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already has an open cash register
        var existingRegisters = await _cashRegisterRepository.GetAllAsync(cancellationToken);
        var openRegister = existingRegisters.FirstOrDefault(cr =>
            cr.UserId == request.UserId && cr.Status == CashRegisterStatus.Open);

        if (openRegister != null)
        {
            throw new InvalidOperationException("User already has an open cash register");
        }

        // Create new cash register
        var cashRegister = new Domain.Entities.CashRegister
        {
            UserId = request.UserId,
            OpeningBalance = request.OpeningBalance,
            OpeningDate = DateTime.UtcNow,
            Status = CashRegisterStatus.Open,
            Notes = request.Notes
        };

        await _cashRegisterRepository.AddAsync(cashRegister, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CashRegisterDto
        {
            Id = cashRegister.Id,
            OpeningDate = cashRegister.OpeningDate,
            ClosingDate = cashRegister.ClosingDate,
            OpeningBalance = cashRegister.OpeningBalance,
            ClosingBalance = cashRegister.ClosingBalance,
            ExpectedBalance = cashRegister.ExpectedBalance,
            Difference = cashRegister.Difference,
            Status = cashRegister.Status.ToString(),
            Notes = cashRegister.Notes,
            UserId = cashRegister.UserId
        };
    }
}
