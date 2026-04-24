using Ceramix.Domain.Entities;
using Ceramix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ceramix.Application.Services;

public class EnrollmentService
{
    private readonly CeramixDbContext _context;

    public EnrollmentService(CeramixDbContext context)
    {
        _context = context;
    }

    public async Task<List<Enrollment>> GetAll()
        => await _context.Enrollments.ToListAsync();

    public async Task Enroll(Guid studentId, Guid sessionId)
    {
        var exists = await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.SessionId == sessionId);

        if (exists)
            throw new Exception("Already enrolled");

        var enrollment = new Enrollment(studentId, sessionId);
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
    }
}
