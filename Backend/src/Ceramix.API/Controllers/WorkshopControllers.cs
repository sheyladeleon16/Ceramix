using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ceramix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkshopsController : ControllerBase
{
    private readonly IWorkshopService _service;
    public WorkshopsController(IWorkshopService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkshopDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkshopDto>> GetById(Guid id, CancellationToken ct)
    {
        var w = await _service.GetByIdAsync(id, ct);
        return w is null ? NotFound() : Ok(w);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<WorkshopDto>>> Search([FromQuery] string title, CancellationToken ct) =>
        Ok(await _service.SearchByTitleAsync(title, ct));

    [HttpGet("instructor/{instructorId:guid}")]
    public async Task<ActionResult<IEnumerable<WorkshopDto>>> GetByInstructor(Guid instructorId, CancellationToken ct) =>
        Ok(await _service.GetByInstructorAsync(instructorId, ct));

    [HttpPost]
    public async Task<ActionResult<WorkshopDto>> Create([FromBody] CreateWorkshopDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<WorkshopDto>> Update(Guid id, [FromBody] UpdateWorkshopDto dto, CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await _service.DeactivateAsync(id, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
