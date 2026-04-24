using Ceramix.Domain.Enums;

namespace Ceramix.Domain.Entities;

public class Student : Person
{
    public SkillLevel Level { get; private set; }
    public string EmergencyContact { get; private set; }
    public string Notes { get; private set; }

    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    public Student(string fullName, string email, string phone,
                   DateTime dateOfBirth, SkillLevel level = SkillLevel.Beginner,
                   string emergencyContact = "", string notes = "")
        : base(fullName, email, phone, dateOfBirth)
    {
        Level = level;
        EmergencyContact = emergencyContact;
        Notes = notes;
    }

    public string GetProfileSummary() =>
        $"{FullName} | Nivel: {Level} | {Email}";

    public string GetProfileSummary(bool includeAge) =>
        includeAge
            ? $"{FullName} | Nivel: {Level} | Edad: {GetAge()} | {Email}"
            : GetProfileSummary();

    public void UpdateLevel(SkillLevel level)
    {
        Level = level;
        MarkAsUpdated();
    }

    public int GetActiveEnrollmentsCount() =>
        _enrollments.Count(e => e.Status == EnrollmentStatus.Active);

    public override string GetSummary() => GetProfileSummary(includeAge: true);
}
