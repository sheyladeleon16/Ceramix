using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ceramix.API.Controllers;

public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;
    public EnrollmentsController(IEnrollmentService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EnrollmentDto>> GetById(Guid id, CancellationToken ct)
    {
        var e = await _service.GetByIdAsync(id, ct);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpGet("workshop/{workshopId:guid}")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetByWorkshop(Guid workshopId, CancellationToken ct) =>
        Ok(await _service.GetByWorkshopAsync(workshopId, ct));

    [HttpGet("student/{studentId:guid}")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetByStudent(Guid studentId, CancellationToken ct) =>
        Ok(await _service.GetByStudentAsync(studentId, ct));

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> Create([FromBody] CreateEnrollmentDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/confirm")]
    public async Task<ActionResult<EnrollmentDto>> Confirm(Guid id, CancellationToken ct) =>
        Ok(await _service.ConfirmAsync(id, ct));

    [HttpPatch("{id:guid}/cancel")]
    public async Task<ActionResult<EnrollmentDto>> Cancel(Guid id, [FromBody] CancelEnrollmentDto dto, CancellationToken ct) =>
        Ok(await _service.CancelAsync(id, dto, ct));

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<EnrollmentDto>> Complete(Guid id, CancellationToken ct) =>
        Ok(await _service.CompleteAsync(id, ct));
}