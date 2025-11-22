using CashFlowSystem.Application.DTOs.Reports;
using CashFlowSystem.Application.Queries.Reports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IMediator mediator, ILogger<ReportsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Generate a cash flow report
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ReportDto>> GenerateReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? userId,
        [FromQuery] string? type,
        [FromQuery] Guid? categoryId)
    {
        try
        {
            var query = new GenerateReportQuery
            {
                StartDate = startDate,
                EndDate = endDate,
                UserId = userId,
                Type = type,
                CategoryId = categoryId
            };

            var report = await _mediator.Send(query);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            return StatusCode(500, new { message = "An error occurred while generating the report" });
        }
    }
}
