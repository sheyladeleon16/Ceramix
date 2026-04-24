public class Firing : BaseEntity
{
    public string Name { get; private set; }
    public FiringType Type { get; private set; }
    public FiringStatus Status { get; private set; }

    public double KilnTemperatureCelsius { get; private set; }
    public int DurationHours { get; private set; }

    public DateTime? PlannedStartDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }

    public Guid InstructorId { get; private set; }
    public Instructor? Instructor { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<FiringPiece> _firingPieces = new();
    public IReadOnlyCollection<FiringPiece> FiringPieces => _firingPieces.AsReadOnly();
    public int PieceCount => _firingPieces.Count;

    // ── Constructor ───────────────────────────────────────────────
    public Firing(string name, FiringType type, double kilnTemperatureCelsius,
                  int durationHours, Guid instructorId, DateTime? plannedStartDate = null)
    {
        if (kilnTemperatureCelsius <= 0)
            throw new ArgumentException("La temperatura debe ser positiva.");
        if (durationHours <= 0)
            throw new ArgumentException("La duración debe ser positiva.");

        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        KilnTemperatureCelsius = kilnTemperatureCelsius;
        DurationHours = durationHours;
        InstructorId = instructorId;
        PlannedStartDate = plannedStartDate;
        Status = FiringStatus.Pending;
    }

    // ── Business rules ────────────────────────────────────────────

    /// <summary>
    /// Solo se pueden agregar piezas secas (CanBeFired == true).
    /// Regla de negocio principal del dominio.
    /// </summary>
    public void AddPiece(Piece piece)
    {
        if (Status != FiringStatus.Pending)
            throw new InvalidOperationException("Solo se pueden añadir piezas a horneos pendientes.");
        if (!piece.CanBeFired())
            throw new InvalidOperationException(
                $"La pieza '{piece.Name}' no está lista para hornear. Estado actual: {piece.Status}.");
        if (_firingPieces.Any(fp => fp.PieceId == piece.Id))
            throw new InvalidOperationException("La pieza ya está incluida en este horneo.");

        _firingPieces.Add(new FiringPiece(Id, piece.Id));
        MarkAsUpdated();
    }

    public void RemovePiece(Guid pieceId)
    {
        if (Status != FiringStatus.Pending)
            throw new InvalidOperationException("No se pueden remover piezas de un horneo en curso o completado.");
        var fp = _firingPieces.FirstOrDefault(p => p.PieceId == pieceId)
            ?? throw new KeyNotFoundException("Pieza no encontrada en este horneo.");
        _firingPieces.Remove(fp);
        MarkAsUpdated();
    }

    public void Start()
    {
        if (Status != FiringStatus.Pending)
            throw new InvalidOperationException("El horneo ya fue iniciado o completado.");
        if (!_firingPieces.Any())
            throw new InvalidOperationException("El horneo debe tener al menos una pieza.");

        Status = FiringStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void Finish(string? notes = null)
    {
        if (Status != FiringStatus.InProgress)
            throw new InvalidOperationException("Solo se puede finalizar un horneo en progreso.");

        Status = FiringStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        Notes = notes;
        MarkAsUpdated();
    }

    public void Cancel(string reason)
    {
        if (Status == FiringStatus.Completed)
            throw new InvalidOperationException("No se puede cancelar un horneo completado.");
        Status = FiringStatus.Cancelled;
        Notes = reason;
        MarkAsUpdated();
    }

    public TimeSpan? GetActualDuration() =>
        ActualStartDate.HasValue && ActualEndDate.HasValue
            ? ActualEndDate - ActualStartDate
            : null;

    // ── Overloading ───────────────────────────────────────────────
    public string GetInfo() => $"{Name} [{Status}] — {PieceCount} piezas";

    public string GetInfo(bool detailed) =>
        detailed
            ? $"{Name} | Tipo: {Type} | Temp: {KilnTemperatureCelsius}°C | " +
              $"Duración: {DurationHours}h | Piezas: {PieceCount} | Estado: {Status}"
            : GetInfo();

    public override string GetSummary() => GetInfo(detailed: true);
}

/// <summary>Tabla de unión entre Firing y Piece (muchos-a-muchos).</summary>
public class FiringPiece : BaseEntity
{
    public Guid FiringId { get; private set; }
    public Guid PieceId { get; private set; }
    public Piece? Piece { get; private set; }
    public string? ResultNotes { get; private set; }
    public bool WasSuccessful { get; private set; }

    public FiringPiece(Guid firingId, Guid pieceId)
    {
        FiringId = firingId;
        PieceId = pieceId;
        WasSuccessful = true;
    }

    public void SetResult(bool wasSuccessful, string? notes)
    {
        WasSuccessful = wasSuccessful;
        ResultNotes = notes;
        MarkAsUpdated();
    }

    public override string GetSummary() =>
        $"FiringPiece: Firing {FiringId} → Pieza {PieceId} [{(WasSuccessful ? "OK" : "Fallida")}]";
}
