using Ceramix.Domain.Entities;
using Ceramix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ceramix.Application.Services;

public class PieceService
{
    private readonly CeramixDbContext _context;

    public PieceService(CeramixDbContext context)
    {
        _context = context;
    }

    public async Task<List<Piece>> GetAll()
        => await _context.Pieces.ToListAsync();

    public async Task<Piece> Create(string name, Guid studentId)
    {
        var piece = new Piece(name, studentId);
        _context.Pieces.Add(piece);
        await _context.SaveChangesAsync();
        return piece;
    }

    public async Task<bool> Finish(Guid id)
    {
        var piece = await _context.Pieces.FindAsync(id);
        if (piece == null) return false;

        piece.Finish();
        await _context.SaveChangesAsync();
        return true;
    }
}
