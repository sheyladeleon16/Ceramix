using Ceramix.Domain.Entities;
using Ceramix.Domain.Enums;
using Ceramix.Infrastructure.Data;

namespace Ceramix.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(CeramixDbContext context)
    {
        if (context.Instructors.Any()) return;

        var instructor1 = new Instructor("María González", "maria@ceramix.com", "809-555-0101",
            new DateTime(1985, 3, 12), "Wheel Throwing", 10, "Ceramista profesional con talleres internacionales.");
        var instructor2 = new Instructor("Carlos Méndez", "carlos@ceramix.com", "809-555-0102",
            new DateTime(1979, 7, 25), "Hand Building & Sculpture", 15, "Escultor y ceramista con más de 15 años.");

        await context.Instructors.AddRangeAsync(instructor1, instructor2);
        await context.SaveChangesAsync();

        var workshop1 = new Workshop("Introducción al Torno", "Aprende las bases del torno cerámico.",
            8, 1500m, WorkshopCategory.WheelThrowing, instructor1.Id);
        var workshop2 = new Workshop("Escultura en Cerámica", "Técnicas avanzadas de modelado a mano.",
            10, 1200m, WorkshopCategory.HandBuilding, instructor2.Id);
        var workshop3 = new Workshop("Cerámica para Niños", "Taller lúdico para niños de 6 a 12 años.",
            12, 800m, WorkshopCategory.Children, instructor1.Id);

        await context.Workshops.AddRangeAsync(workshop1, workshop2, workshop3);
        await context.SaveChangesAsync();

        var schedule1 = new Schedule(workshop1.Id,
            DateTime.UtcNow.AddDays(3).Date.AddHours(10),
            DateTime.UtcNow.AddDays(3).Date.AddHours(13),
            "Sala A — Planta Baja");
        var schedule2 = new Schedule(workshop2.Id,
            DateTime.UtcNow.AddDays(5).Date.AddHours(14),
            DateTime.UtcNow.AddDays(5).Date.AddHours(17),
            "Sala B — Primer Piso");

        await context.Schedules.AddRangeAsync(schedule1, schedule2);
        await context.SaveChangesAsync();

        var student1 = new Student("Ana Torres", "ana@email.com", "809-555-0201",
            new DateTime(1998, 5, 20), SkillLevel.Beginner, "Pedro Torres 809-555-9001");
        var student2 = new Student("Luis Reyes", "luis@email.com", "809-555-0202",
            new DateTime(1995, 11, 8), SkillLevel.Intermediate, "Rosa Reyes 809-555-9002");

        await context.Students.AddRangeAsync(student1, student2);
        await context.SaveChangesAsync();

        var enrollment1 = new Enrollment(workshop1.Id, student1.Id);
        enrollment1.Confirm();
        var enrollment2 = new Enrollment(workshop2.Id, student2.Id);
        enrollment2.Confirm();

        await context.Enrollments.AddRangeAsync(enrollment1, enrollment2);
        await context.SaveChangesAsync();

        var payment1 = new Payment(enrollment1.Id, 1500m, PaymentMethod.CreditCard);
        payment1.MarkAsPaid("TXN-001-2025");
        var payment2 = new Payment(enrollment2.Id, 1200m, PaymentMethod.BankTransfer);

        await context.Payments.AddRangeAsync(payment1, payment2);
        await context.SaveChangesAsync();
    }
}
