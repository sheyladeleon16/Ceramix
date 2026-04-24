using Ceramix.Domain.Enums;

namespace Ceramix.Domain.Entities;
public class Piece : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid StudentId { get; private set; }
    public Student? Student { get; private set; }
    public Guid WorkshopId { get; private set; }
    public Workshop? Workshop { get; private set; }

    public PieceStatus Status { get; private set; }
    public CeramicTechnique Technique { get; private set; }

    public DateTime? DryingStartDate { get; private set; }
    public DateTime? DryingEndDate { get; private set; }
    public DateTime? CompletionDate { get; private set; }

    public string? GlazeColor { get; private set; }
    public string? InstructorNotes { get; private set; }
    public double WeightGrams { get; private set; }

    private readonly List<FiringPiece> _firingPieces = new();
    public IReadOnlyCollection<FiringPiece> FiringPieces => _firingPieces.AsReadOnly();

    public Piece(string name, string description, Guid studentId,
                 Guid workshopId, CeramicTechnique technique, double weightGrams = 0)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        StudentId = studentId;
        WorkshopId = workshopId;
        Technique = technique;
        WeightGrams = weightGrams >= 0 ? weightGrams : 0;
        Status = PieceStatus.InProgress;
    }

    public bool CanBeFired() => Status == PieceStatus.Dried;

    public void StartDrying()
    {
        if (Status != PieceStatus.InProgress)
            throw new InvalidOperationException("Solo se pueden secar piezas en progreso.");
        Status = PieceStatus.Drying;
        DryingStartDate = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void MarkAsDried()
    {
        if (Status != PieceStatus.Drying)
            throw new InvalidOperationException("La pieza debe estar en fase de secado.");
        Status = PieceStatus.Dried;
        DryingEndDate = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void MarkAsFired()
    {
        if (!CanBeFired())
            throw new InvalidOperationException("La pieza debe estar seca antes de hornear.");
        Status = PieceStatus.Fired;
        MarkAsUpdated();
    }

    public void MarkAsCompleted(string? instructorNotes = null)
    {
        if (Status != PieceStatus.Fired)
            throw new InvalidOperationException("Solo se pueden completar piezas horneadas.");
        Status = PieceStatus.Completed;
        CompletionDate = DateTime.UtcNow;
        InstructorNotes = instructorNotes;
        MarkAsUpdated();
    }

    public void SetGlaze(string glazeColor)
    {
        GlazeColor = glazeColor;
        MarkAsUpdated();
    }

    public string GetInfo() => $"{Name} [{Status}] — {Technique}";

    public string GetInfo(bool detailed) =>
        detailed
            ? $"{Name} | Técnica: {Technique} | Peso: {WeightGrams}g | Estado: {Status} | " +
              $"Esmalte: {GlazeColor ?? "Sin esmalte"}"
            : GetInfo();

    public string GetInfo(bool detailed, bool includeNotes) =>
        detailed && includeNotes && InstructorNotes is not null
            ? $"{GetInfo(true)} | Notas: {InstructorNotes}"
            : GetInfo(detailed);

    public override string GetSummary() => GetInfo(detailed: true);
}
