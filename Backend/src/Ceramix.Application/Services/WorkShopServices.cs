using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Ceramix.Domain.Entities;
using Ceramix.Domain.Interfaces;

namespace Ceramix.Application.Services;

public class WorkshopService : IWorkshopService
{
    private readonly IRepository<Workshop> _workshopRepo;
    private readonly IRepository<Instructor> _instructorRepo;

    public WorkshopService(IRepository<Workshop> workshopRepo,
                           IRepository<Instructor> instructorRepo)
    {
        _workshopRepo = workshopRepo;
        _instructorRepo = instructorRepo;
    }

    public async Task<IEnumerable<WorkshopDto>> GetAllAsync(CancellationToken ct = default)
    {
        var workshops = await _workshopRepo.GetAllAsync(ct);
        return workshops.Select(MapToDto);
    }

    public async Task<WorkshopDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var w = await _workshopRepo.GetByIdAsync(id, ct);
        return w is null ? null : MapToDto(w);
    }

    public async Task<IEnumerable<WorkshopDto>> GetByInstructorAsync(Guid instructorId, CancellationToken ct = default)
    {
        var workshops = await _workshopRepo.FindAsync(w => w.InstructorId == instructorId, ct);
        return workshops.Select(MapToDto);
    }

    public async Task<IEnumerable<WorkshopDto>> SearchByTitleAsync(string title, CancellationToken ct = default)
    {
        var workshops = await _workshopRepo.FindAsync(
            w => w.Title.Contains(title, StringComparison.OrdinalIgnoreCase), ct);
        return workshops.Select(MapToDto);
    }

    public async Task<WorkshopDto> CreateAsync(CreateWorkshopDto dto, CancellationToken ct = default)
    {
        var instructorExists = await _instructorRepo.ExistsAsync(i => i.Id == dto.InstructorId, ct);
        if (!instructorExists)
            throw new KeyNotFoundException($"Instructor {dto.InstructorId} no encontrado.");

        var workshop = new Workshop(dto.Title, dto.Description, dto.MaxStudents,
                                    dto.Price, dto.Category, dto.InstructorId);
        await _workshopRepo.AddAsync(workshop, ct);
        return MapToDto(workshop);
    }

    public async Task<WorkshopDto> UpdateAsync(Guid id, UpdateWorkshopDto dto, CancellationToken ct = default)
    {
        var workshop = await _workshopRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Taller {id} no encontrado.");

        workshop.Update(dto.Title, dto.Description, dto.MaxStudents, dto.Price, dto.Category);
        await _workshopRepo.UpdateAsync(workshop, ct);
        return MapToDto(workshop);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        var workshop = await _workshopRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Taller {id} no encontrado.");
        workshop.Deactivate();
        await _workshopRepo.UpdateAsync(workshop, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var workshop = await _workshopRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Taller {id} no encontrado.");
        await _workshopRepo.DeleteAsync(workshop, ct);
    }

    private static WorkshopDto MapToDto(Workshop w) => new(
        w.Id, w.Title, w.Description, w.MaxStudents,
        w.GetAvailableSpots(), w.Price, w.Category.ToString(),
        w.IsActive, w.InstructorId,
        w.Instructor?.FullName, w.CreatedAt);
}
