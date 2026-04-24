using Ceramix.Domain.Enums;

namespace Ceramix.Domain.Entities;

public class Workshop : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int MaxStudents { get; private set; }
    public decimal Price { get; private set; }
    public WorkshopCategory Category { get; private set; }
    public bool IsActive { get; private set; }
    public Guid InstructorId { get; private set; }
    public Instructor? Instructor { get; private set; }

    private readonly List<Enrollment> _enrollments = new();
    private readonly List<Schedule> _schedules = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();

    public Workshop(string title, string description, int maxStudents,
                    decimal price, WorkshopCategory category, Guid instructorId)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        MaxStudents = maxStudents > 0 ? maxStudents : throw new ArgumentException("MaxStudents must be > 0");
        Price = price >= 0 ? price : throw new ArgumentException("Price cannot be negative");
        Category = category;
        InstructorId = instructorId;
        IsActive = true;
    }

    public bool HasAvailableSpots() =>
        _enrollments.Count(e => e.Status == EnrollmentStatus.Active) < MaxStudents;

    public int GetAvailableSpots() =>
        MaxStudents - _enrollments.Count(e => e.Status == EnrollmentStatus.Active);

    public void AddEnrollment(Enrollment enrollment)
    {
        if (!HasAvailableSpots())
            throw new InvalidOperationException("El taller está lleno.");
        _enrollments.Add(enrollment);
    }

    public void Update(string title, string description, int maxStudents,
                       decimal price, WorkshopCategory category)
    {
        Title = title;
        Description = description;
        MaxStudents = maxStudents;
        Price = price;
        Category = category;
        MarkAsUpdated();
    }

    public void Deactivate() { IsActive = false; MarkAsUpdated(); }
    public void Activate()   { IsActive = true;  MarkAsUpdated(); }

    public override string GetSummary() =>
        $"Taller: {Title} ({_enrollments.Count}/{MaxStudents} alumnos) — {Category}";
}
