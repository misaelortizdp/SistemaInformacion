using CashFlowSystem.Application.Commands.PaymentMethods;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.Queries.PaymentMethods;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentMethodsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentMethodsController> _logger;

    public PaymentMethodsController(IMediator mediator, ILogger<PaymentMethodsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all payment methods
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetAll()
    {
        try
        {
            var query = new GetAllPaymentMethodsQuery();
            var paymentMethods = await _mediator.Send(query);
            return Ok(paymentMethods);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment methods");
            return StatusCode(500, new { message = "An error occurred while retrieving payment methods" });
        }
    }

    /// <summary>
    /// Create a new payment method
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PaymentMethodDto>> Create([FromBody] CreatePaymentMethodCommand command)
    {
        try
        {
            var paymentMethod = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = paymentMethod.Id }, paymentMethod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment method");
            return StatusCode(500, new { message = "An error occurred while creating payment method" });
        }
    }

    /// <summary>
    /// Update an existing payment method
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<PaymentMethodDto>> Update(Guid id, [FromBody] UpdatePaymentMethodCommand command)
    {
        try
        {
            if (id != command.Id)
                return BadRequest(new { message = "ID mismatch" });

            var paymentMethod = await _mediator.Send(command);
            return Ok(paymentMethod);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Payment method not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment method");
            return StatusCode(500, new { message = "An error occurred while updating payment method" });
        }
    }

    /// <summary>
    /// Delete a payment method
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeletePaymentMethodCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(new { message = "Payment method not found" });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting payment method");
            return StatusCode(500, new { message = "An error occurred while deleting payment method" });
        }
    }
}
