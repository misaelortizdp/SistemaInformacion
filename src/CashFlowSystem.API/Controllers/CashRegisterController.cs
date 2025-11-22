using CashFlowSystem.Application.Commands.CashRegister;
using CashFlowSystem.Application.Queries.CashRegister;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CashRegisterController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CashRegisterController> _logger;

    public CashRegisterController(IMediator mediator, ILogger<CashRegisterController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get open cash register for a user
    /// </summary>
    [HttpGet("open/{userId}")]
    public async Task<ActionResult<CashRegisterDto>> GetOpenCashRegister(Guid userId)
    {
        try
        {
            var query = new GetOpenCashRegisterQuery { UserId = userId };
            var cashRegister = await _mediator.Send(query);

            if (cashRegister == null)
            {
                return NotFound(new { message = "No open cash register found for this user" });
            }

            return Ok(cashRegister);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving open cash register for user: {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while retrieving the cash register" });
        }
    }

    /// <summary>
    /// Open a new cash register
    /// </summary>
    [HttpPost("open")]
    public async Task<ActionResult<CashRegisterDto>> OpenCashRegister([FromBody] OpenCashRegisterCommand command)
    {
        try
        {
            var cashRegister = await _mediator.Send(command);
            return Ok(cashRegister);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error opening cash register for user: {UserId}", command.UserId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening cash register for user: {UserId}", command.UserId);
            return StatusCode(500, new { message = "An error occurred while opening the cash register" });
        }
    }

    /// <summary>
    /// Close a cash register
    /// </summary>
    [HttpPost("close/{id}")]
    public async Task<ActionResult<CashRegisterDto>> CloseCashRegister(Guid id, [FromBody] CloseCashRegisterCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var cashRegister = await _mediator.Send(command);
            return Ok(cashRegister);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error closing cash register: {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing cash register: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while closing the cash register" });
        }
    }
}
