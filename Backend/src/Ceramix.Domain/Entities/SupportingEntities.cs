using Ceramix.Domain.Enums;

namespace Ceramix.Domain.Entities;

public class Material : BaseEntity
{
    public string Name { get; private set; }
    public MaterialType Type { get; private set; }
    public double Quantity { get; private set; }
    public string Unit { get; private set; }          // "kg", "litros", "unidades"
    public double MinimumStock { get; private set; }
    public string? Supplier { get; private set; }

    private readonly List<MaterialUsage> _usages = new();
    public IReadOnlyCollection<MaterialUsage> Usages => _usages.AsReadOnly();

    public Material(string name, MaterialType type, double quantity,
                    string unit, double minimumStock = 0, string? supplier = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        Quantity = quantity >= 0 ? quantity : throw new ArgumentException("Quantity must be >= 0.");
        Unit = unit ?? throw new ArgumentNullException(nameof(unit));
        MinimumStock = minimumStock;
        Supplier = supplier;
    }

    public bool IsLowStock() => Quantity <= MinimumStock;

    public void Consume(double amount, Guid sessionId)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        if (amount > Quantity) throw new InvalidOperationException("Stock insuficiente.");
        Quantity -= amount;
        _usages.Add(new MaterialUsage(Id, sessionId, amount));
        MarkAsUpdated();
    }

    public void Restock(double amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        Quantity += amount;
        MarkAsUpdated();
    }

    public string GetStockInfo() => $"{Name}: {Quantity} {Unit}";
    public string GetStockInfo(bool includeAlert) =>
        includeAlert && IsLowStock()
            ? $"{GetStockInfo()} ⚠ Stock bajo (mín: {MinimumStock} {Unit})"
            : GetStockInfo();

    public override string GetSummary() => GetStockInfo(includeAlert: true);
}

public class MaterialUsage : BaseEntity
{
    public Guid MaterialId { get; private set; }
    public Guid SessionId { get; private set; }
    public double AmountUsed { get; private set; }
    public DateTime UsedAt { get; private set; }

    public MaterialUsage(Guid materialId, Guid sessionId, double amountUsed)
    {
        MaterialId = materialId;
        SessionId = sessionId;
        AmountUsed = amountUsed;
        UsedAt = DateTime.UtcNow;
    }

    public override string GetSummary() =>
        $"Uso: {AmountUsed} de Material {MaterialId} en Sesión {SessionId}";
}

public class Progress : BaseEntity
{
    public Guid EnrollmentId { get; private set; }
    public Enrollment? Enrollment { get; private set; }
    public Guid SessionId { get; private set; }
    public Session? Session { get; private set; }

    public int SkillScore { get; private set; }     
    public int AttitudeScore { get; private set; }  
    public string Comments { get; private set; }
    public DateTime EvaluatedAt { get; private set; }

    public Progress(Guid enrollmentId, Guid sessionId,
                    int skillScore, int attitudeScore, string comments)
    {
        ValidateScore(skillScore, nameof(skillScore));
        ValidateScore(attitudeScore, nameof(attitudeScore));

        EnrollmentId = enrollmentId;
        SessionId = sessionId;
        SkillScore = skillScore;
        AttitudeScore = attitudeScore;
        Comments = comments ?? string.Empty;
        EvaluatedAt = DateTime.UtcNow;
    }

    private static void ValidateScore(int score, string name)
    {
        if (score < 1 || score > 10)
            throw new ArgumentOutOfRangeException(name, "La puntuación debe estar entre 1 y 10.");
    }

    public double GetAverageScore() => (SkillScore + AttitudeScore) / 2.0;

    public override string GetSummary() =>
        $"Avance: Técnica={SkillScore}/10 | Actitud={AttitudeScore}/10 | Prom={GetAverageScore():F1}";
}
public class Delivery : BaseEntity
{
    public Guid StudentId { get; private set; }
    public Student? Student { get; private set; }
    public Guid WorkshopId { get; private set; }
    public DateTime DeliveryDate { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public string? RecipientSignature { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<Guid> _pieceIds = new();
    public IReadOnlyCollection<Guid> PieceIds => _pieceIds.AsReadOnly();
    public int PieceCount => _pieceIds.Count;

    public Delivery(Guid studentId, Guid workshopId, IEnumerable<Guid> pieceIds)
    {
        StudentId = studentId;
        WorkshopId = workshopId;
        _pieceIds.AddRange(pieceIds ?? throw new ArgumentNullException(nameof(pieceIds)));
        if (!_pieceIds.Any())
            throw new ArgumentException("Una entrega debe incluir al menos una pieza.");
        DeliveryDate = DateTime.UtcNow;
        Status = DeliveryStatus.Pending;
    }

    public void Confirm(string recipientSignature)
    {
        Status = DeliveryStatus.Delivered;
        RecipientSignature = recipientSignature;
        MarkAsUpdated();
    }

    public void AddNote(string note) { Notes = note; MarkAsUpdated(); }

    public override string GetSummary() =>
        $"Entrega: {PieceCount} piezas → Estudiante {StudentId} [{Status}] {DeliveryDate:dd/MM/yyyy}";
}