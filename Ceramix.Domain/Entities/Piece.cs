namespace Ceramix.Domain.Entities;

public class Piece
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public Guid StudentId { get; set; }

    public Piece(string name, Guid studentId)
    {
        Id = Guid.NewGuid();
        Name = name;
        StudentId = studentId;
        Status = "In Progress";
    }

    public void Finish() => Status = "Finished";
}
