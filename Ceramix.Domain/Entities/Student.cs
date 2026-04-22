namespace Ceramix.Domain.Entities;

public class Student : Person
{
    public DateTime EnrollmentDate { get; set; }

    public List<Piece> Pieces { get; set; } = new();

    public Student(string name, string email) 
        : base(name, email)
    {
        EnrollmentDate = DateTime.Now;
    }

    public void AddPiece(Piece piece)
    {
        Pieces.Add(piece);
    }
}