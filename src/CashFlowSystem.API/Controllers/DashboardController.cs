using CashFlowSystem.Application.DTOs.Dashboard;
using CashFlowSystem.Application.Queries.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard data for a specific date range
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboardData(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? userId)
    {
        try
        {
            // Default to current month if not specified
            var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = endDate ?? DateTime.Now;

            var query = new GetDashboardDataQuery
            {
                StartDate = start,
                EndDate = end,
                UserId = userId
            };

            var dashboard = await _mediator.Send(query);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard data");
            return StatusCode(500, new { message = "An error occurred while retrieving dashboard data" });
        }
    }
}
