namespace Ceramix.Application.DTOs;

public class SessionDTO
{
    public Guid Id { get; set; }
    public Guid WorkshopId { get; set; }
    public string? WorkshopTitle { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Topic { get; set; }
    public string Location { get; set; }
    public bool IsCompleted { get; set; }
    public string? Summary { get; set; }
    public int AttendanceCount { get; set; }
    public double DurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record CreateSessionDto(
    Guid WorkshopId,
    DateTime StartTime,
    DateTime EndTime,
    string Topic,
    string Location
);

public record RescheduleSessionDto(DateTime NewStart, DateTime NewEnd);

public record RecordAttendanceDto(
    Guid StudentId,
    bool WasPresent,
    string? Notes
);
