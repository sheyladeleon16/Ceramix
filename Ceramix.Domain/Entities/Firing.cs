namespace Ceramix.Domain.Entities;

public class Firing
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public List<Piece> Pieces { get; set; } = new();

    public Firing()
    {
        Id = Guid.NewGuid();
        Date = DateTime.Now;
    }

    public void AddPiece(Piece piece)
    {
        if (piece.Status != "Finished")
            throw new Exception("Only finished pieces can be fired.");

        Pieces.Add(piece);
    }
}