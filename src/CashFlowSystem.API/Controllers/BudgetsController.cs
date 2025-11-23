using CashFlowSystem.Application.Commands.Budgets;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.Queries.Budgets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BudgetsController> _logger;

    public BudgetsController(IMediator mediator, ILogger<BudgetsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all budgets for a user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetUserBudgets(
        [FromQuery] Guid userId,
        [FromQuery] bool? isActive)
    {
        try
        {
            var query = new GetUserBudgetsQuery { UserId = userId, IsActive = isActive };
            var budgets = await _mediator.Send(query);
            return Ok(budgets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budgets");
            return StatusCode(500, new { message = "An error occurred while retrieving budgets" });
        }
    }

    /// <summary>
    /// Create a new budget
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BudgetDto>> Create([FromBody] CreateBudgetCommand command)
    {
        try
        {
            var budget = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUserBudgets), new { userId = budget.UserId }, budget);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating budget");
            return StatusCode(500, new { message = "An error occurred while creating budget" });
        }
    }

    /// <summary>
    /// Update an existing budget
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetDto>> Update(Guid id, [FromBody] UpdateBudgetCommand command)
    {
        try
        {
            if (id != command.Id)
                return BadRequest(new { message = "ID mismatch" });

            var budget = await _mediator.Send(command);
            return Ok(budget);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Budget not found" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget");
            return StatusCode(500, new { message = "An error occurred while updating budget" });
        }
    }

    /// <summary>
    /// Delete a budget
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteBudgetCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(new { message = "Budget not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting budget");
            return StatusCode(500, new { message = "An error occurred while deleting budget" });
        }
    }
}
