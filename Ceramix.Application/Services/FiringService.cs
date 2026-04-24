using Ceramix.Domain.Entities;
using Ceramix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ceramix.Application.Services;

public class FiringService
{
    private readonly CeramixDbContext _context;

    public FiringService(CeramixDbContext context)
    {
        _context = context;
    }

    public async Task<List<Firing>> GetAll()
        => await _context.Firings.Include(f => f.Pieces).ToListAsync();

    public async Task<Firing> Create()
    {
        var firing = new Firing();
        _context.Firings.Add(firing);
        await _context.SaveChangesAsync();
        return firing;
    }

    public async Task AddPiece(Guid firingId, Guid pieceId)
    {
        var firing = await _context.Firings
            .Include(f => f.Pieces)
            .FirstOrDefaultAsync(f => f.Id == firingId);

        var piece = await _context.Pieces.FindAsync(pieceId);

        if (firing == null || piece == null)
            throw new Exception("Invalid data");

        firing.AddPiece(piece);
        await _context.SaveChangesAsync();
    }
}