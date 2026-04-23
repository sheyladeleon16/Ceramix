namespace Ceramix.Domain.Entities;

public class Student : Person
{
    public List<Piece> Pieces { get; set; } = new();

    public Student(string name, string email) : base(name, email) { }
}