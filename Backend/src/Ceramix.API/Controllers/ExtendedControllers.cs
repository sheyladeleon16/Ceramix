using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ceramix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    public ReportsController(IReportService service) => _service = service;

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboard(CancellationToken ct) =>
        Ok(await _service.GetDashboardStatsAsync(ct));

    [HttpGet("workshops")]
    public async Task<ActionResult<IEnumerable<WorkshopReportDto>>> GetWorkshopReport(CancellationToken ct) =>
        Ok(await _service.GetWorkshopReportAsync(ct));
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _svc;
    public SessionsController(ISessionService svc) => _svc = svc;

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetUpcoming(CancellationToken ct) =>
        Ok(await _svc.GetUpcomingAsync(ct));

    [HttpGet("workshop/{workshopId:guid}")]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetByWorkshop(Guid workshopId, CancellationToken ct) =>
        Ok(await _svc.GetByWorkshopAsync(workshopId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SessionDto>> GetById(Guid id, CancellationToken ct)
    {
        var s = await _svc.GetByIdAsync(id, ct);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpPost]
    public async Task<ActionResult<SessionDto>> Create([FromBody] CreateSessionDto dto, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/reschedule")]
    public async Task<ActionResult<SessionDto>> Reschedule(Guid id, [FromBody] RescheduleSessionDto dto, CancellationToken ct) =>
        Ok(await _svc.RescheduleAsync(id, dto, ct));

    [HttpPost("{id:guid}/attendance")]
    public async Task<IActionResult> RecordAttendance(Guid id, [FromBody] RecordAttendanceDto dto, CancellationToken ct)
    {
        await _svc.RecordAttendanceAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<SessionDto>> Complete(Guid id, [FromBody] CompleteSessionDto dto, CancellationToken ct) =>
        Ok(await _svc.CompleteAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}

// ─── Firings ─────────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FiringsController : ControllerBase
{
    private readonly IFiringService _svc;
    public FiringsController(IFiringService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FiringDto>>> GetAll(CancellationToken ct) =>
        Ok(await _svc.GetAllAsync(ct));

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<FiringDto>>> GetPending(CancellationToken ct) =>
        Ok(await _svc.GetPendingAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FiringDto>> GetById(Guid id, CancellationToken ct)
    {
        var f = await _svc.GetByIdAsync(id, ct);
        return f is null ? NotFound() : Ok(f);
    }

    [HttpPost]
    public async Task<ActionResult<FiringDto>> Create([FromBody] CreateFiringDto dto, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id:guid}/pieces")]
    public async Task<ActionResult<FiringDto>> AddPiece(Guid id, [FromBody] AddPieceToFiringDto dto, CancellationToken ct) =>
        Ok(await _svc.AddPieceAsync(id, dto, ct));

    [HttpDelete("{id:guid}/pieces/{pieceId:guid}")]
    public async Task<ActionResult<FiringDto>> RemovePiece(Guid id, Guid pieceId, CancellationToken ct) =>
        Ok(await _svc.RemovePieceAsync(id, pieceId, ct));

    [HttpPatch("{id:guid}/start")]
    public async Task<ActionResult<FiringDto>> Start(Guid id, CancellationToken ct) =>
        Ok(await _svc.StartAsync(id, ct));

    [HttpPatch("{id:guid}/finish")]
    public async Task<ActionResult<FiringDto>> Finish(Guid id, [FromBody] FinishFiringDto dto, CancellationToken ct) =>
        Ok(await _svc.FinishAsync(id, dto, ct));

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromQuery] string reason, CancellationToken ct)
    {
        await _svc.CancelAsync(id, reason, ct);
        return NoContent();
    }
}

