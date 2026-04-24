using Ceramix.Domain.Entities;
using Ceramix.Infrastructure.Persistence;

namespace Ceramix.Application.Services;

public class SessionService
{
    private readonly CeramixDbContext _context;

    public SessionService(CeramixDbContext context)
    {
        _context = context;
    }

    public async Task<List<Session>> GetAll()
        => _context.Sessions.ToList();

    public async Task<Session> Create(string topic, DateTime date)
    {
        var session = new Session(topic, date);
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }
}