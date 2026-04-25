namespace Ceramix.Application.DTOs;

public class PieceDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public Guid WorkshopId { get; set; }
    public string? WorkshopTitle { get; set; }
    public string Status { get; set; }
    public string Technique { get; set; }
    public double WeightGrams { get; set; }
    public string? GlazeColor { get; set; }
    public string? InstructorNotes { get; set; }
    public bool CanBeFired { get; set; }
    public DateTime? DryingEndDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public DateTime CreatedAt { get; set; }
};

public record CreatePieceDto(
    string Name,
    string Description,
    Guid StudentId,
    Guid WorkshopId,
    CeramicTechnique Technique,
    double WeightGrams
);

public record UpdatePieceStatusDto(string Action);  
public record SetGlazeDto(string GlazeColor);
public record CompletePieceDto(string? InstructorNotes);
