using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ceramix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;
    public PaymentsController(IPaymentService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id, CancellationToken ct)
    {
        var p = await _service.GetByIdAsync(id, ct);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpGet("enrollment/{enrollmentId:guid}")]
    public async Task<ActionResult<PaymentDto>> GetByEnrollment(Guid enrollmentId, CancellationToken ct)
    {
        var p = await _service.GetByEnrollmentAsync(enrollmentId, ct);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/confirm")]
    public async Task<ActionResult<PaymentDto>> Confirm(Guid id, [FromBody] ConfirmPaymentDto dto, CancellationToken ct) =>
        Ok(await _service.ConfirmPaymentAsync(id, dto, ct));

    [HttpPatch("{id:guid}/refund")]
    public async Task<ActionResult<PaymentDto>> Refund(Guid id, [FromQuery] string notes, CancellationToken ct) =>
        Ok(await _service.RefundAsync(id, notes, ct));
}
