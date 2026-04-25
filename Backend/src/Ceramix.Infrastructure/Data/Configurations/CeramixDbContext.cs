using Ceramix.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ceramix.Infrastructure.Data;

public class CeramixDbContext : DbContext
{
    public CeramixDbContext(DbContextOptions<CeramixDbContext> options) : base(options) { }

    public DbSet<Workshop> Workshops => Set<Workshop>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CeramixDbContext).Assembly);

        modelBuilder.Entity<Workshop>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Instructor>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Student>().HasQueryFilter(e => !e.IsDeleted);
    }
}
