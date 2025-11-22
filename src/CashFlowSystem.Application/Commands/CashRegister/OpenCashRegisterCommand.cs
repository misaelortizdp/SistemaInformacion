using CashFlowSystem.Application.DTOs;
using MediatR;

namespace CashFlowSystem.Application.Commands.CashRegister;

public class OpenCashRegisterCommand : IRequest<CashRegisterDto>
{
    public Guid UserId { get; set; }
    public decimal OpeningBalance { get; set; }
    public string? Notes { get; set; }
}

public class CashRegisterDto
{
    public Guid Id { get; set; }
    public DateTime OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal? ClosingBalance { get; set; }
    public decimal? ExpectedBalance { get; set; }
    public decimal? Difference { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid UserId { get; set; }
}
