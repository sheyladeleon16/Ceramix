namespace Ceramix.Domain.Entities;

public class Schedule : BaseEntity
{
    public Guid WorkshopId { get; private set; }
    public Workshop? Workshop { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Location { get; private set; }
    public bool IsCancelled { get; private set; }
    public string? CancellationNote { get; private set; }

    public Schedule(Guid workshopId, DateTime startTime, DateTime endTime, string location)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be after StartTime.");

        WorkshopId = workshopId;
        StartTime = startTime;
        EndTime = endTime;
        Location = location ?? throw new ArgumentNullException(nameof(location));
    }

    public TimeSpan GetDuration() => EndTime - StartTime;

    public bool IsUpcoming() => StartTime > DateTime.UtcNow;

    public bool OverlapsWith(Schedule other) =>
        StartTime < other.EndTime && EndTime > other.StartTime;

    public void CancelSession(string note)
    {
        IsCancelled = true;
        CancellationNote = note;
        MarkAsUpdated();
    }

    public void Reschedule(DateTime newStart, DateTime newEnd)
    {
        if (newEnd <= newStart) throw new ArgumentException("EndTime must be after StartTime.");
        StartTime = newStart;
        EndTime = newEnd;
        MarkAsUpdated();
    }

    public override string GetSummary() =>
        $"Sesión: {StartTime:dd/MM/yyyy HH:mm} → {EndTime:HH:mm} en {Location}";
}
