namespace Ceramix.Application.DTOs;

public class StudentDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Level { get; set; }
    public string EmergencyContact { get; set; }
    public string Notes { get; set; }
    public int Age { get; set; }
    public int ActiveEnrollments { get; set; }
    public DateTime CreatedAt { get; set; }
}
public record CreateStudentDto(
    string FullName,
    string Email,
    string Phone,
    DateTime DateOfBirth,
    SkillLevel Level,
    string EmergencyContact,
    string Notes
);

public record UpdateStudentDto(
    string FullName,
    string Email,
    string Phone,
    SkillLevel Level,
    string EmergencyContact,
    string Notes
);