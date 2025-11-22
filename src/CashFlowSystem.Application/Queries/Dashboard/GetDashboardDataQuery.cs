using CashFlowSystem.Application.DTOs.Dashboard;
using MediatR;

namespace CashFlowSystem.Application.Queries.Dashboard;

public class GetDashboardDataQuery : IRequest<DashboardDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? UserId { get; set; }
}
