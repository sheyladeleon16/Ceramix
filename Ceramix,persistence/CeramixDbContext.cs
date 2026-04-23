using Microsoft.EntityFrameworkCore;
using Ceramix.Domain.Entities;

namespace Ceramix.Infrastructure.Persistence;

public class CeramixDbContext : DbContext
{
    public CeramixDbContext(DbContextOptions<CeramixDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Piece> Pieces { get; set; }
    public DbSet<Firing> Firings { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relaciones básicas
        modelBuilder.Entity<Student>()
            .HasMany(s => s.Pieces)
            .WithOne()
            .HasForeignKey(p => p.StudentId);

        modelBuilder.Entity<Firing>()
            .HasMany(f => f.Pieces);

        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.StudentId, e.SessionId })
            .IsUnique(); // evita duplicados
    }
}