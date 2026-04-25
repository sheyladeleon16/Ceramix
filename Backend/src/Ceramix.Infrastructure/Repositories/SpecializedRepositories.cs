using Ceramix.Domain.Entities;
using Ceramix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ceramix.Infrastructure.Repositories;

public class WorkshopRepository : Repository<Workshop>
{
    public WorkshopRepository(CeramixDbContext context) : base(context) { }

    public async Task<Workshop?> GetWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        await _dbSet
            .Include(w => w.Instructor)
            .Include(w => w.Enrollments).ThenInclude(e => e.Student)
            .Include(w => w.Schedules)
            .FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<IEnumerable<Workshop>> GetAllWithInstructorAsync(CancellationToken ct = default) =>
        await _dbSet
            .Include(w => w.Instructor)
            .AsNoTracking()
            .ToListAsync(ct);
}

public class EnrollmentRepository : Repository<Enrollment>
{
    public EnrollmentRepository(CeramixDbContext context) : base(context) { }

    public async Task<IEnumerable<Enrollment>> GetByWorkshopWithDetailsAsync(
        Guid workshopId, CancellationToken ct = default) =>
        await _dbSet
            .Include(e => e.Student)
            .Include(e => e.Workshop)
            .Include(e => e.Payment)
            .Where(e => e.WorkshopId == workshopId)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IEnumerable<Enrollment>> GetByStudentWithDetailsAsync(
        Guid studentId, CancellationToken ct = default) =>
        await _dbSet
            .Include(e => e.Workshop).ThenInclude(w => w!.Instructor)
            .Include(e => e.Payment)
            .Where(e => e.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync(ct);
}

public class ScheduleRepository : Repository<Schedule>
{
    public ScheduleRepository(CeramixDbContext context) : base(context) { }

    public async Task<IEnumerable<Schedule>> GetUpcomingWithWorkshopAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(s => s.Workshop).ThenInclude(w => w!.Instructor)
            .Where(s => s.StartTime > now && !s.IsCancelled)
            .OrderBy(s => s.StartTime)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
