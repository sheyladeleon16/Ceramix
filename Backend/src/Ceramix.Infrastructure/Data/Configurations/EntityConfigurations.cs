using Ceramix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ceramix.Infrastructure.Data.Configurations;

public class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Title).IsRequired().HasMaxLength(200);
        builder.Property(w => w.Description).HasMaxLength(2000);
        builder.Property(w => w.Price).HasColumnType("decimal(18,2)");
        builder.Property(w => w.Category).HasConversion<string>();

        builder.HasOne(w => w.Instructor)
               .WithMany(i => i.Workshops)
               .HasForeignKey(w => w.InstructorId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(w => w.Enrollments)
               .WithOne(e => e.Workshop)
               .HasForeignKey(e => e.WorkshopId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Schedules)
               .WithOne(s => s.Workshop)
               .HasForeignKey(s => s.WorkshopId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Workshops");
    }
}

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.FullName).IsRequired().HasMaxLength(150);
        builder.Property(i => i.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(i => i.Email).IsUnique();
        builder.Property(i => i.Phone).HasMaxLength(30);
        builder.Property(i => i.Specialty).IsRequired().HasMaxLength(150);
        builder.Property(i => i.Bio).HasMaxLength(1000);
        builder.ToTable("Instructors");
    }
}

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.FullName).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(s => s.Email).IsUnique();
        builder.Property(s => s.Phone).HasMaxLength(30);
        builder.Property(s => s.Level).HasConversion<string>();
        builder.Property(s => s.EmergencyContact).HasMaxLength(200);
        builder.Property(s => s.Notes).HasMaxLength(1000);
        builder.ToTable("Students");
    }
}

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasConversion<string>();
        builder.Property(e => e.CancellationReason).HasMaxLength(500);

        builder.HasOne(e => e.Student)
               .WithMany(s => s.Enrollments)
               .HasForeignKey(e => e.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Payment)
               .WithOne(p => p.Enrollment)
               .HasForeignKey<Payment>(p => p.EnrollmentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Enrollments");
    }
}

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Location).IsRequired().HasMaxLength(200);
        builder.Property(s => s.CancellationNote).HasMaxLength(500);
        builder.ToTable("Schedules");
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        builder.Property(p => p.Status).HasConversion<string>();
        builder.Property(p => p.Method).HasConversion<string>();
        builder.Property(p => p.TransactionReference).HasMaxLength(300);
        builder.Property(p => p.Notes).HasMaxLength(500);
        builder.ToTable("Payments");
    }
}
