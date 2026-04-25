using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ceramix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    public StudentsController(IStudentService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentDto>> GetById(Guid id, CancellationToken ct)
    {
        var s = await _service.GetByIdAsync(id, ct);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<StudentDto>>> Search([FromQuery] string term, CancellationToken ct) =>
        Ok(await _service.SearchAsync(term, ct));

    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StudentDto>> Update(Guid id, [FromBody] UpdateStudentDto dto, CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}