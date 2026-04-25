using Ceramix.Domain.Entities;

namespace Ceramix.Application.DTOs;

public class EnrollmentDTO
{
    public Guid Id { get; set; }
    public Guid WorkshopId { get; set; }
    public string? WorkshopTitle { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; }
    public string? CancellationReason { get; set; }
}
public record CreateEnrollmentDto(Guid WorkshopId, Guid StudentId);

public record CancelEnrollmentDto(string Reason);
