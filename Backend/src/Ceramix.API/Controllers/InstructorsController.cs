using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ceramix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _service;
    public InstructorsController(IInstructorService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstructorDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InstructorDto>> GetById(Guid id, CancellationToken ct)
    {
        var i = await _service.GetByIdAsync(id, ct);
        return i is null ? NotFound() : Ok(i);
    }

    [HttpPost]
    public async Task<ActionResult<InstructorDto>> Create([FromBody] CreateInstructorDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InstructorDto>> Update(Guid id, [FromBody] UpdateInstructorDto dto, CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}

