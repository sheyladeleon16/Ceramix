namespace Ceramix.Application.DTOs;
public record MaterialDto(
    Guid Id,
    string Name,
    string Type,
    double Quantity,
    string Unit,
    double MinimumStock,
    bool IsLowStock,
    string? Supplier
);

public record CreateMaterialDto(
    string Name,
    MaterialType Type,
    double Quantity,
    string Unit,
    double MinimumStock,
    string? Supplier
);
public record ConsumeStockDto(double Amount, Guid SessionId);
public record RestockDto(double Amount);

//──────Progress───────────────────────────────────────────────
public record ProgressDto(
    Guid Id,
    Guid EnrollmentId,
    Guid SessionId,
    string? SessionTopic,
    int SkillScore,
    int AttitudeScore,
    double AverageScore,
    string Comments,
    DateTime EvaluatedAt
);

public record CreateProgressDto(
    Guid EnrollmentId,
    Guid SessionId,
    int SkillScore,
    int AttitudeScore,
    string Comments
);

// ─── Delivery ─────────────────────────────────────────────────────
public record DeliveryDto(
    Guid Id,
    Guid StudentId,
    string? StudentName,
    Guid WorkshopId,
    int PieceCount,
    string Status,
    DateTime DeliveryDate,
    string? RecipientSignature,
    string? Notes
);

public record CreateDeliveryDto(Guid StudentId, Guid WorkshopId, List<Guid> PieceIds);
public record ConfirmDeliveryDto(string RecipientSignature);

// ─── Workshop DTOs ───────────────────────────────────────────────
public record WorkshopDto(
    Guid Id,
    string Title,
    string Description,
    int MaxStudents,
    int AvailableSpots,
    decimal Price,
    string Category,
    bool IsActive,
    Guid InstructorId,
    string? InstructorName,
    DateTime CreatedAt
);

public record CreateWorkshopDto(
    string Title,
    string Description,
    int MaxStudents,
    decimal Price,
    WorkshopCategory Category,
    Guid InstructorId
);

public record UpdateWorkshopDto(
    string Title,
    string Description,
    int MaxStudents,
    decimal Price,
    WorkshopCategory Category
);

// ─── Instructor DTOs ─────────────────────────────────────────────
public record InstructorDto(
    Guid Id,
    string FullName,
    string Email,
    string Phone,
    string Specialty,
    int YearsOfExperience,
    string Bio,
    int Age,
    DateTime CreatedAt
);

public record CreateInstructorDto(
    string FullName,
    string Email,
    string Phone,
    DateTime DateOfBirth,
    string Specialty,
    int YearsOfExperience,
    string Bio
);

public record UpdateInstructorDto(
    string FullName,
    string Email,
    string Phone,
    string Specialty,
    int YearsOfExperience,
    string Bio
);
public record ScheduleDto(
    Guid Id,
    Guid WorkshopId,
    string? WorkshopTitle,
    DateTime StartTime,
    DateTime EndTime,
    string Location,
    bool IsCancelled,
    string? CancellationNote,
    double DurationMinutes
);

public record CreateScheduleDto(
    Guid WorkshopId,
    DateTime StartTime,
    DateTime EndTime,
    string Location
);

public record RescheduleDto(DateTime NewStart, DateTime NewEnd);

// ─── Payment DTOs ────────────────────────────────────────────────
public record PaymentDto(
    Guid Id,
    Guid EnrollmentId,
    decimal Amount,
    string Status,
    string Method,
    string? TransactionReference,
    DateTime? PaidAt,
    string? Notes
);

public record CreatePaymentDto(
    Guid EnrollmentId,
    decimal Amount,
    PaymentMethod Method
);

public record ConfirmPaymentDto(string TransactionReference);

// ─── Reports DTOs ────────────────────────────────────────────────
public record DashboardStatsDto(
    int TotalWorkshops,
    int ActiveWorkshops,
    int TotalStudents,
    int TotalInstructors,
    int TotalEnrollments,
    int PendingPayments,
    decimal TotalRevenue
);

public record WorkshopReportDto(
    Guid WorkshopId,
    string Title,
    string InstructorName,
    int TotalEnrolled,
    int MaxStudents,
    decimal OccupancyRate,
    decimal TotalRevenue
);

// ─── Shared ──────────────────────────────────────────────────────
public record PagedResultDto<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record ApiErrorDto(string Message, IEnumerable<string>? Details = null);
