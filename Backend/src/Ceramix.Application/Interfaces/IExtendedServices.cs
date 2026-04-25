using Ceramix.Application.DTOs;

namespace Ceramix.Application.Interfaces;

public interface IPieceService
{
    Task<IEnumerable<PieceDto>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<PieceDto>> GetByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<IEnumerable<PieceDto>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct = default);
    Task<IEnumerable<PieceDto>> GetReadyForFiringAsync(CancellationToken ct = default);
    Task<PieceDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PieceDto> CreateAsync(CreatePieceDto dto, CancellationToken ct = default);
    Task<PieceDto> StartDryingAsync(Guid id, CancellationToken ct = default);
    Task<PieceDto> MarkAsDriedAsync(Guid id, CancellationToken ct = default);
    Task<PieceDto> SetGlazeAsync(Guid id, SetGlazeDto dto, CancellationToken ct = default);
    Task<PieceDto> CompleteAsync(Guid id, CompletePieceDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface ISessionService
{
    Task<IEnumerable<SessionDto>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct = default);
    Task<IEnumerable<SessionDto>> GetUpcomingAsync(CancellationToken ct = default);
    Task<SessionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SessionDto> CreateAsync(CreateSessionDto dto, CancellationToken ct = default);
    Task<SessionDto> RescheduleAsync(Guid id, RescheduleSessionDto dto, CancellationToken ct = default);
    Task RecordAttendanceAsync(Guid id, RecordAttendanceDto dto, CancellationToken ct = default);
    Task<SessionDto> CompleteAsync(Guid id, CompleteSessionDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IFiringService
{
    Task<IEnumerable<FiringDto>> GetAllAsync(CancellationToken ct = default);
    Task<FiringDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<FiringDto>> GetPendingAsync(CancellationToken ct = default);
    Task<FiringDto> CreateAsync(CreateFiringDto dto, CancellationToken ct = default);
    Task<FiringDto> AddPieceAsync(Guid firingId, AddPieceToFiringDto dto, CancellationToken ct = default);
    Task<FiringDto> RemovePieceAsync(Guid firingId, Guid pieceId, CancellationToken ct = default);
    Task<FiringDto> StartAsync(Guid id, CancellationToken ct = default);
    Task<FiringDto> FinishAsync(Guid id, FinishFiringDto dto, CancellationToken ct = default);
    Task<FiringDto> CancelAsync(Guid id, string reason, CancellationToken ct = default);
}

public interface IMaterialService
{
    Task<IEnumerable<MaterialDto>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<MaterialDto>> GetLowStockAsync(CancellationToken ct = default);
    Task<MaterialDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MaterialDto> CreateAsync(CreateMaterialDto dto, CancellationToken ct = default);
    Task<MaterialDto> ConsumeAsync(Guid id, ConsumeStockDto dto, CancellationToken ct = default);
    Task<MaterialDto> RestockAsync(Guid id, RestockDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IProgressService
{
    Task<IEnumerable<ProgressDto>> GetByEnrollmentAsync(Guid enrollmentId, CancellationToken ct = default);
    Task<ProgressDto> CreateAsync(CreateProgressDto dto, CancellationToken ct = default);
}

public interface IDeliveryService
{
    Task<IEnumerable<DeliveryDto>> GetByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<DeliveryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<DeliveryDto> CreateAsync(CreateDeliveryDto dto, CancellationToken ct = default);
    Task<DeliveryDto> ConfirmAsync(Guid id, ConfirmDeliveryDto dto, CancellationToken ct = default);
}
