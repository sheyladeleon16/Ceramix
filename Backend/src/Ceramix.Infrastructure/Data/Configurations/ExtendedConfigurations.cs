using Ceramix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ceramix.Infrastructure.Data.Configurations;

public class PieceConfiguration : IEntityTypeConfiguration<Piece>
{
    public void Configure(EntityTypeBuilder<Piece> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Status).HasConversion<string>();
        builder.Property(p => p.Technique).HasConversion<string>();
        builder.Property(p => p.GlazeColor).HasMaxLength(100);
        builder.Property(p => p.InstructorNotes).HasMaxLength(500);

        builder.HasOne(p => p.Student)
               .WithMany()
               .HasForeignKey(p => p.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Workshop)
               .WithMany()
               .HasForeignKey(p => p.WorkshopId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Pieces");
    }
}

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Topic).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Location).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Summary).HasMaxLength(2000);

        builder.HasOne(s => s.Workshop)
               .WithMany()
               .HasForeignKey(s => s.WorkshopId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Attendances)
               .WithOne()
               .HasForeignKey(a => a.SessionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Sessions");
    }
}

public class SessionAttendanceConfiguration : IEntityTypeConfiguration<SessionAttendance>
{
    public void Configure(EntityTypeBuilder<SessionAttendance> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Notes).HasMaxLength(500);
        builder.HasIndex(a => new { a.SessionId, a.StudentId }).IsUnique();
        builder.ToTable("SessionAttendances");
    }
}

public class FiringConfiguration : IEntityTypeConfiguration<Firing>
{
    public void Configure(EntityTypeBuilder<Firing> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired().HasMaxLength(200);
        builder.Property(f => f.Type).HasConversion<string>();
        builder.Property(f => f.Status).HasConversion<string>();
        builder.Property(f => f.Notes).HasMaxLength(1000);

        builder.HasOne(f => f.Instructor)
               .WithMany()
               .HasForeignKey(f => f.InstructorId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.FiringPieces)
               .WithOne()
               .HasForeignKey(fp => fp.FiringId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Firings");
    }
}

public class FiringPieceConfiguration : IEntityTypeConfiguration<FiringPiece>
{
    public void Configure(EntityTypeBuilder<FiringPiece> builder)
    {
        builder.HasKey(fp => fp.Id);
        builder.HasIndex(fp => new { fp.FiringId, fp.PieceId }).IsUnique();
        builder.Property(fp => fp.ResultNotes).HasMaxLength(500);

        builder.HasOne(fp => fp.Piece)
               .WithMany(p => p.FiringPieces)
               .HasForeignKey(fp => fp.PieceId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("FiringPieces");
    }
}

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Type).HasConversion<string>();
        builder.Property(m => m.Unit).IsRequired().HasMaxLength(30);
        builder.Property(m => m.Supplier).HasMaxLength(200);
        builder.ToTable("Materials");
    }
}

public class ProgressConfiguration : IEntityTypeConfiguration<Progress>
{
    public void Configure(EntityTypeBuilder<Progress> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Comments).HasMaxLength(1000);

        builder.HasOne(p => p.Enrollment)
               .WithMany()
               .HasForeignKey(p => p.EnrollmentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Session)
               .WithMany()
               .HasForeignKey(p => p.SessionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Progress");
    }
}

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Status).HasConversion<string>();
        builder.Property(d => d.RecipientSignature).HasMaxLength(300);
        builder.Property(d => d.Notes).HasMaxLength(500);

        builder.Property(d => d.PieceIds)
               .HasConversion(
                   ids => System.Text.Json.JsonSerializer.Serialize(ids, (System.Text.Json.JsonSerializerOptions?)null),
                   json => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(json, (System.Text.Json.JsonSerializerOptions?)null)!
               );

        builder.ToTable("Deliveries");
    }
}
