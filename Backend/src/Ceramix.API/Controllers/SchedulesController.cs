using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ceramix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _service;
    public SchedulesController(IScheduleService service) => _service = service;

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetUpcoming(CancellationToken ct) =>
        Ok(await _service.GetUpcomingAsync(ct));

    [HttpGet("workshop/{workshopId:guid}")]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetByWorkshop(Guid workshopId, CancellationToken ct) =>
        Ok(await _service.GetByWorkshopAsync(workshopId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScheduleDto>> GetById(Guid id, CancellationToken ct)
    {
        var s = await _service.GetByIdAsync(id, ct);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleDto>> Create([FromBody] CreateScheduleDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/reschedule")]
    public async Task<ActionResult<ScheduleDto>> Reschedule(Guid id, [FromBody] RescheduleDto dto, CancellationToken ct) =>
        Ok(await _service.RescheduleAsync(id, dto, ct));

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelSession(Guid id, [FromQuery] string note, CancellationToken ct)
    {
        await _service.CancelSessionAsync(id, note, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}