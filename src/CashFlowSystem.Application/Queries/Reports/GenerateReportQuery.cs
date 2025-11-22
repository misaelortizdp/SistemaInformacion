using CashFlowSystem.Application.DTOs.Reports;
using MediatR;

namespace CashFlowSystem.Application.Queries.Reports;

public class GenerateReportQuery : IRequest<ReportDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? UserId { get; set; }
    public string? Type { get; set; } // "Income", "Expense", or null for all
    public Guid? CategoryId { get; set; }
}
