using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Commands.CashRegister;

public class CloseCashRegisterCommand : IRequest<CashRegisterDto>
{
    public Guid Id { get; set; }
    public decimal ClosingBalance { get; set; }
    public string? Notes { get; set; }
}
