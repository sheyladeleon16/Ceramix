using Ceramix.Domain.Enums;

namespace Ceramix.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid WorkshopId { get; private set; }
    public Guid StudentId { get; private set; }
    public Workshop? Workshop { get; private set; }
    public Student? Student { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public Payment? Payment { get; private set; }

    public Enrollment(Guid workshopId, Guid studentId)
    {
        WorkshopId = workshopId;
        StudentId = studentId;
        EnrollmentDate = DateTime.UtcNow;
        Status = EnrollmentStatus.Pending;
    }

    public void Confirm()
    {
        if (Status != EnrollmentStatus.Pending)
            throw new InvalidOperationException("Solo se pueden confirmar inscripciones pendientes.");
        Status = EnrollmentStatus.Active;
        MarkAsUpdated();
    }

    public void Cancel(string reason)
    {
        Status = EnrollmentStatus.Cancelled;
        CancellationReason = reason;
        MarkAsUpdated();
    }

    public void Complete()
    {
        if (Status != EnrollmentStatus.Active)
            throw new InvalidOperationException("Solo se pueden completar inscripciones activas.");
        Status = EnrollmentStatus.Completed;
        MarkAsUpdated();
    }

    public override string GetSummary() =>
        $"Inscripción: Estudiante {StudentId} → Taller {WorkshopId} [{Status}]";
}
