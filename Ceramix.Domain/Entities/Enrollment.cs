namespace Ceramix.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid SessionId { get; set; }

    public Enrollment(Guid studentId, Guid sessionId)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        SessionId = sessionId;
    }
}