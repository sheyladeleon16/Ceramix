public class Session : BaseEntity
{
    public Guid WorkshopId { get; private set; }
    public Workshop? Workshop { get; private set; }

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Topic { get; private set; }
    public string Location { get; private set; }
    public bool IsCompleted { get; private set; }
    public string? Summary { get; private set; }

    private readonly List<SessionAttendance> _attendances = new();
    public IReadOnlyCollection<SessionAttendance> Attendances => _attendances.AsReadOnly();

    public int AttendanceCount => _attendances.Count(a => a.WasPresent);

    public Session(Guid workshopId, DateTime startTime, DateTime endTime,
                   string topic, string location)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime debe ser posterior a StartTime.");

        WorkshopId = workshopId;
        StartTime = startTime;
        EndTime = endTime;
        Topic = topic ?? throw new ArgumentNullException(nameof(topic));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        IsCompleted = false;
    }

    public TimeSpan GetDuration() => EndTime - StartTime;

    public bool IsUpcoming() => StartTime > DateTime.UtcNow;
    public bool IsInProgress() => StartTime <= DateTime.UtcNow && EndTime >= DateTime.UtcNow;

    public void RecordAttendance(Guid studentId, bool wasPresent)
    {
        var existing = _attendances.FirstOrDefault(a => a.StudentId == studentId);
        if (existing is not null)
        {
            existing.UpdatePresence(wasPresent);
            return;
        }
        _attendances.Add(new SessionAttendance(Id, studentId, wasPresent));
    }

    public void Complete(string summary)
    {
        IsCompleted = true;
        Summary = summary;
        MarkAsUpdated();
    }

    public void Reschedule(DateTime newStart, DateTime newEnd)
    {
        if (newEnd <= newStart)
            throw new ArgumentException("EndTime debe ser posterior a StartTime.");
        if (IsCompleted)
            throw new InvalidOperationException("No se puede reprogramar una sesión completada.");
        StartTime = newStart;
        EndTime = newEnd;
        MarkAsUpdated();
    }

    public string GetSummaryLine() =>
        $"Sesión: {Topic} | {StartTime:dd/MM HH:mm} | {AttendanceCount} asistentes";

    public string GetSummaryLine(bool includeLocation) =>
        includeLocation
            ? $"{GetSummaryLine()} | {Location}"
            : GetSummaryLine();

    public override string GetSummary() => GetSummaryLine(includeLocation: true);
}

public class SessionAttendance : BaseEntity
{
    public Guid SessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public bool WasPresent { get; private set; }
    public string? Notes { get; private set; }

    public SessionAttendance(Guid sessionId, Guid studentId, bool wasPresent, string? notes = null)
    {
        SessionId = sessionId;
        StudentId = studentId;
        WasPresent = wasPresent;
        Notes = notes;
    }

    public void UpdatePresence(bool wasPresent)
    {
        WasPresent = wasPresent;
        MarkAsUpdated();
    }

    public override string GetSummary() =>
        $"Asistencia — Estudiante {StudentId}: {(WasPresent ? "Presente" : "Ausente")}";
}
