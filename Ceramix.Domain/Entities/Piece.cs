namespace Ceramix.Domain.Entities;

public class Piece
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Material { get; set; }
    public string Status { get; set; }

    public Guid StudentId { get; set; }

    // Constructor
    public Piece(string name, string material)
    {
        Id = Guid.NewGuid();
        Name = name;
        Material = material;
        Status = "In Progress";
    }

    // Método
    public void MarkAsFinished()
    {
        Status = "Finished";
    }
}