using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ceramix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PiecesController : ControllerBase
{
    private readonly IPieceService _svc;
    public PiecesController(IPieceService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PieceDto>>> GetAll(CancellationToken ct) =>
        Ok(await _svc.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PieceDto>> GetById(Guid id, CancellationToken ct)
    {
        var p = await _svc.GetByIdAsync(id, ct);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<ActionResult<IEnumerable<PieceDto>>> GetByStudent(Guid studentId, CancellationToken ct) =>
        Ok(await _svc.GetByStudentAsync(studentId, ct));

    [HttpGet("workshop/{workshopId:guid}")]
    public async Task<ActionResult<IEnumerable<PieceDto>>> GetByWorkshop(Guid workshopId, CancellationToken ct) =>
        Ok(await _svc.GetByWorkshopAsync(workshopId, ct));

    [HttpGet("ready-for-firing")]
    public async Task<ActionResult<IEnumerable<PieceDto>>> GetReadyForFiring(CancellationToken ct) =>
        Ok(await _svc.GetReadyForFiringAsync(ct));

    [HttpPost]
    public async Task<ActionResult<PieceDto>> Create([FromBody] CreatePieceDto dto, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/start-drying")]
    public async Task<ActionResult<PieceDto>> StartDrying(Guid id, CancellationToken ct) =>
        Ok(await _svc.StartDryingAsync(id, ct));

    [HttpPatch("{id:guid}/mark-dried")]
    public async Task<ActionResult<PieceDto>> MarkDried(Guid id, CancellationToken ct) =>
        Ok(await _svc.MarkAsDriedAsync(id, ct));

    [HttpPatch("{id:guid}/glaze")]
    public async Task<ActionResult<PieceDto>> SetGlaze(Guid id, [FromBody] SetGlazeDto dto, CancellationToken ct) =>
        Ok(await _svc.SetGlazeAsync(id, dto, ct));

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<PieceDto>> Complete(Guid id, [FromBody] CompletePieceDto dto, CancellationToken ct) =>
        Ok(await _svc.CompleteAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
