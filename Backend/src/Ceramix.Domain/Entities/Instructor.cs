namespace Ceramix.Domain.Entities;

public class Instructor : Person
{
    public string Specialty { get; set; }

    public Instructor(string name, string email, string specialty)
        : base(name, email)
    {
        Specialty = specialty;
    }
}