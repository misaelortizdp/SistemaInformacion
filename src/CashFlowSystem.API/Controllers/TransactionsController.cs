using CashFlowSystem.Application.Commands.Transactions;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.Queries.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all transactions with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll(
        [FromQuery] Guid? userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? type)
    {
        try
        {
            var query = new GetAllTransactionsQuery
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                Type = type
            };

            var transactions = await _mediator.Send(query);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions");
            return StatusCode(500, new { message = "An error occurred while retrieving transactions" });
        }
    }

    /// <summary>
    /// Get a transaction by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetById(Guid id)
    {
        try
        {
            var query = new GetTransactionByIdQuery { Id = id };
            var transaction = await _mediator.Send(query);
            return Ok(transaction);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Transaction not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the transaction" });
        }
    }

    /// <summary>
    /// Create a new transaction
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create([FromBody] CreateTransactionCommand command)
    {
        try
        {
            var transaction = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error creating transaction");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return StatusCode(500, new { message = "An error occurred while creating the transaction" });
        }
    }

    /// <summary>
    /// Update an existing transaction
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TransactionDto>> Update(Guid id, [FromBody] UpdateTransactionCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var transaction = await _mediator.Send(command);
            return Ok(transaction);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error updating transaction: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the transaction" });
        }
    }

    /// <summary>
    /// Delete a transaction
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteTransactionCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(new { message = "Transaction not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the transaction" });
        }
    }
}
