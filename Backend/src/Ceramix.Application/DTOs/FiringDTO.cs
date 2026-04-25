namespace Ceramix.Application.DTOs;

public record FiringDto(
    Guid Id,
    string Name,
    string Type,
    string Status,
    double KilnTemperatureCelsius,
    int DurationHours,
    int PieceCount,
    Guid InstructorId,
    string? InstructorName,
    DateTime? PlannedStartDate,
    DateTime? ActualStartDate,
    DateTime? ActualEndDate,
    string? Notes,
    DateTime CreatedAt
);

public record CreateFiringDto(
    string Name,
    FiringType Type,
    double KilnTemperatureCelsius,
    int DurationHours,
    Guid InstructorId,
    DateTime? PlannedStartDate
);

public record AddPieceToFiringDto(Guid PieceId);
public record SetFiringResultDto(Guid PieceId, bool WasSuccessful, string? Notes);
public record FinishFiringDto(string? Notes);
