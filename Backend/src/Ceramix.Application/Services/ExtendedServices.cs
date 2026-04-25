using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Ceramix.Domain.Entities;
using Ceramix.Domain.Interfaces;

namespace Ceramix.Application.Services;

// ─── PieceService ─────────────────────────────────────────────────
public class PieceService : IPieceService
{
    private readonly IRepository<Piece> _pieceRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Workshop> _workshopRepo;

    public PieceService(IRepository<Piece> pieceRepo,
                        IRepository<Student> studentRepo,
                        IRepository<Workshop> workshopRepo)
    {
        _pieceRepo   = pieceRepo;
        _studentRepo = studentRepo;
        _workshopRepo = workshopRepo;
    }

    public async Task<IEnumerable<PieceDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _pieceRepo.GetAllAsync(ct)).Select(MapToDto);

    public async Task<IEnumerable<PieceDto>> GetByStudentAsync(Guid studentId, CancellationToken ct = default) =>
        (await _pieceRepo.FindAsync(p => p.StudentId == studentId, ct)).Select(MapToDto);

    public async Task<IEnumerable<PieceDto>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct = default) =>
        (await _pieceRepo.FindAsync(p => p.WorkshopId == workshopId, ct)).Select(MapToDto);

    public async Task<IEnumerable<PieceDto>> GetReadyForFiringAsync(CancellationToken ct = default) =>
        (await _pieceRepo.FindAsync(p => p.CanBeFired(), ct)).Select(MapToDto);

    public async Task<PieceDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _pieceRepo.GetByIdAsync(id, ct);
        return p is null ? null : MapToDto(p);
    }

    public async Task<PieceDto> CreateAsync(CreatePieceDto dto, CancellationToken ct = default)
    {
        var studentExists  = await _studentRepo.ExistsAsync(s => s.Id == dto.StudentId, ct);
        var workshopExists = await _workshopRepo.ExistsAsync(w => w.Id == dto.WorkshopId, ct);
        if (!studentExists)  throw new KeyNotFoundException("Estudiante no encontrado.");
        if (!workshopExists) throw new KeyNotFoundException("Taller no encontrado.");

        var piece = new Piece(dto.Name, dto.Description, dto.StudentId,
                              dto.WorkshopId, dto.Technique, dto.WeightGrams);
        await _pieceRepo.AddAsync(piece, ct);
        return MapToDto(piece);
    }

    public async Task<PieceDto> StartDryingAsync(Guid id, CancellationToken ct = default)
    {
        var piece = await GetOrThrow(id, ct);
        piece.StartDrying();
        await _pieceRepo.UpdateAsync(piece, ct);
        return MapToDto(piece);
    }

    public async Task<PieceDto> MarkAsDriedAsync(Guid id, CancellationToken ct = default)
    {
        var piece = await GetOrThrow(id, ct);
        piece.MarkAsDried();
        await _pieceRepo.UpdateAsync(piece, ct);
        return MapToDto(piece);
    }

    public async Task<PieceDto> SetGlazeAsync(Guid id, SetGlazeDto dto, CancellationToken ct = default)
    {
        var piece = await GetOrThrow(id, ct);
        piece.SetGlaze(dto.GlazeColor);
        await _pieceRepo.UpdateAsync(piece, ct);
        return MapToDto(piece);
    }

    public async Task<PieceDto> CompleteAsync(Guid id, CompletePieceDto dto, CancellationToken ct = default)
    {
        var piece = await GetOrThrow(id, ct);
        piece.MarkAsCompleted(dto.InstructorNotes);
        await _pieceRepo.UpdateAsync(piece, ct);
        return MapToDto(piece);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var piece = await GetOrThrow(id, ct);
        piece.SoftDelete();
        await _pieceRepo.UpdateAsync(piece, ct);
    }

    private async Task<Piece> GetOrThrow(Guid id, CancellationToken ct) =>
        await _pieceRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Pieza {id} no encontrada.");

    private static PieceDto MapToDto(Piece p) => new(
        p.Id, p.Name, p.Description,
        p.StudentId, p.Student?.FullName,
        p.WorkshopId, p.Workshop?.Title,
        p.Status.ToString(), p.Technique.ToString(),
        p.WeightGrams, p.GlazeColor, p.InstructorNotes,
        p.CanBeFired(), p.DryingEndDate, p.CompletionDate, p.CreatedAt);
}

// ─── FiringService ────────────────────────────────────────────────
public class FiringService : IFiringService
{
    private readonly IRepository<Firing> _firingRepo;
    private readonly IRepository<Piece>  _pieceRepo;

    public FiringService(IRepository<Firing> firingRepo,
                         IRepository<Piece>  pieceRepo)
    {
        _firingRepo = firingRepo;
        _pieceRepo  = pieceRepo;
    }

    public async Task<IEnumerable<FiringDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _firingRepo.GetAllAsync(ct)).Select(MapToDto);

    public async Task<FiringDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var f = await _firingRepo.GetByIdAsync(id, ct);
        return f is null ? null : MapToDto(f);
    }

    public async Task<IEnumerable<FiringDto>> GetPendingAsync(CancellationToken ct = default) =>
        (await _firingRepo.FindAsync(f => f.Status == Domain.Enums.FiringStatus.Pending, ct))
            .Select(MapToDto);

    public async Task<FiringDto> CreateAsync(CreateFiringDto dto, CancellationToken ct = default)
    {
        var firing = new Firing(dto.Name, dto.Type, dto.KilnTemperatureCelsius,
                                dto.DurationHours, dto.InstructorId, dto.PlannedStartDate);
        await _firingRepo.AddAsync(firing, ct);
        return MapToDto(firing);
    }

    public async Task<FiringDto> AddPieceAsync(Guid firingId, AddPieceToFiringDto dto,
                                                CancellationToken ct = default)
    {
        var firing = await GetOrThrow(firingId, ct);
        var piece  = await _pieceRepo.GetByIdAsync(dto.PieceId, ct)
            ?? throw new KeyNotFoundException($"Pieza {dto.PieceId} no encontrada.");

        firing.AddPiece(piece);         // ← business rule: piece.CanBeFired() checked here
        piece.MarkAsFired();            // transition piece status

        await _firingRepo.UpdateAsync(firing, ct);
        await _pieceRepo.UpdateAsync(piece, ct);
        return MapToDto(firing);
    }

    public async Task<FiringDto> RemovePieceAsync(Guid firingId, Guid pieceId, CancellationToken ct = default)
    {
        var firing = await GetOrThrow(firingId, ct);
        firing.RemovePiece(pieceId);
        await _firingRepo.UpdateAsync(firing, ct);
        return MapToDto(firing);
    }

    public async Task<FiringDto> StartAsync(Guid id, CancellationToken ct = default)
    {
        var firing = await GetOrThrow(id, ct);
        firing.Start();
        await _firingRepo.UpdateAsync(firing, ct);
        return MapToDto(firing);
    }

    public async Task<FiringDto> FinishAsync(Guid id, FinishFiringDto dto, CancellationToken ct = default)
    {
        var firing = await GetOrThrow(id, ct);
        firing.Finish(dto.Notes);
        await _firingRepo.UpdateAsync(firing, ct);
        return MapToDto(firing);
    }

    public async Task<FiringDto> CancelAsync(Guid id, string reason, CancellationToken ct = default)
    {
        var firing = await GetOrThrow(id, ct);
        firing.Cancel(reason);
        await _firingRepo.UpdateAsync(firing, ct);
        return MapToDto(firing);
    }

    private async Task<Firing> GetOrThrow(Guid id, CancellationToken ct) =>
        await _firingRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Horneo {id} no encontrado.");

    private static FiringDto MapToDto(Firing f) => new(
        f.Id, f.Name, f.Type.ToString(), f.Status.ToString(),
        f.KilnTemperatureCelsius, f.DurationHours, f.PieceCount,
        f.InstructorId, f.Instructor?.FullName,
        f.PlannedStartDate, f.ActualStartDate, f.ActualEndDate,
        f.Notes, f.CreatedAt);
}

// ─── SessionService ───────────────────────────────────────────────
public class SessionService : ISessionService
{
    private readonly IRepository<Session>  _sessionRepo;
    private readonly IRepository<Workshop> _workshopRepo;

    public SessionService(IRepository<Session> sessionRepo,
                          IRepository<Workshop> workshopRepo)
    {
        _sessionRepo  = sessionRepo;
        _workshopRepo = workshopRepo;
    }

    public async Task<IEnumerable<SessionDto>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct = default) =>
        (await _sessionRepo.FindAsync(s => s.WorkshopId == workshopId, ct)).Select(MapToDto);

    public async Task<IEnumerable<SessionDto>> GetUpcomingAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return (await _sessionRepo.FindAsync(s => s.StartTime > now && !s.IsCompleted, ct))
            .OrderBy(s => s.StartTime).Select(MapToDto);
    }

    public async Task<SessionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _sessionRepo.GetByIdAsync(id, ct);
        return s is null ? null : MapToDto(s);
    }

    public async Task<SessionDto> CreateAsync(CreateSessionDto dto, CancellationToken ct = default)
    {
        var workshopExists = await _workshopRepo.ExistsAsync(w => w.Id == dto.WorkshopId, ct);
        if (!workshopExists) throw new KeyNotFoundException("Taller no encontrado.");

        var session = new Session(dto.WorkshopId, dto.StartTime, dto.EndTime,
                                  dto.Topic, dto.Location);
        await _sessionRepo.AddAsync(session, ct);
        return MapToDto(session);
    }

    public async Task<SessionDto> RescheduleAsync(Guid id, RescheduleSessionDto dto, CancellationToken ct = default)
    {
        var session = await GetOrThrow(id, ct);
        session.Reschedule(dto.NewStart, dto.NewEnd);
        await _sessionRepo.UpdateAsync(session, ct);
        return MapToDto(session);
    }

    public async Task RecordAttendanceAsync(Guid id, RecordAttendanceDto dto, CancellationToken ct = default)
    {
        var session = await GetOrThrow(id, ct);
        session.RecordAttendance(dto.StudentId, dto.WasPresent);
        await _sessionRepo.UpdateAsync(session, ct);
    }

    public async Task<SessionDto> CompleteAsync(Guid id, CompleteSessionDto dto, CancellationToken ct = default)
    {
        var session = await GetOrThrow(id, ct);
        session.Complete(dto.Summary);
        await _sessionRepo.UpdateAsync(session, ct);
        return MapToDto(session);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var session = await GetOrThrow(id, ct);
        await _sessionRepo.DeleteAsync(session, ct);
    }

    private async Task<Session> GetOrThrow(Guid id, CancellationToken ct) =>
        await _sessionRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Sesión {id} no encontrada.");

    private static SessionDto MapToDto(Session s) => new(
        s.Id, s.WorkshopId, s.Workshop?.Title,
        s.StartTime, s.EndTime, s.Topic, s.Location,
        s.IsCompleted, s.Summary, s.AttendanceCount,
        s.GetDuration().TotalMinutes, s.CreatedAt);
}
