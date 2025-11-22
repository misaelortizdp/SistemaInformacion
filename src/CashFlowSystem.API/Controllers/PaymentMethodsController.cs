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
}
