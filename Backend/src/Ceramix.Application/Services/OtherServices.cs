using Ceramix.Application.DTOs;
using Ceramix.Application.Interfaces;
using Ceramix.Domain.Entities;
using Ceramix.Domain.Enums;
using Ceramix.Domain.Interfaces;

namespace Ceramix.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IRepository<Enrollment> _enrollmentRepo;
    private readonly IRepository<Workshop> _workshopRepo;
    private readonly IRepository<Student> _studentRepo;

    public EnrollmentService(IRepository<Enrollment> enrollmentRepo,
                             IRepository<Workshop> workshopRepo,
                             IRepository<Student> studentRepo)
    {
        _enrollmentRepo = enrollmentRepo;
        _workshopRepo = workshopRepo;
        _studentRepo = studentRepo;
    }

    public async Task<IEnumerable<EnrollmentDto>> GetAllAsync(CancellationToken ct = default)
    {
        var enrollments = await _enrollmentRepo.GetAllAsync(ct);
        return enrollments.Select(MapToDto);
    }

    public async Task<IEnumerable<EnrollmentDto>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct = default)
    {
        var list = await _enrollmentRepo.FindAsync(e => e.WorkshopId == workshopId, ct);
        return list.Select(MapToDto);
    }

    public async Task<IEnumerable<EnrollmentDto>> GetByStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        var list = await _enrollmentRepo.FindAsync(e => e.StudentId == studentId, ct);
        return list.Select(MapToDto);
    }

    public async Task<EnrollmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _enrollmentRepo.GetByIdAsync(id, ct);
        return e is null ? null : MapToDto(e);
    }

    public async Task<EnrollmentDto> CreateAsync(CreateEnrollmentDto dto, CancellationToken ct = default)
    {
        var workshop = await _workshopRepo.GetByIdAsync(dto.WorkshopId, ct)
            ?? throw new KeyNotFoundException("Taller no encontrado.");
        if (!workshop.HasAvailableSpots())
            throw new InvalidOperationException("El taller no tiene cupos disponibles.");

        var studentExists = await _studentRepo.ExistsAsync(s => s.Id == dto.StudentId, ct);
        if (!studentExists) throw new KeyNotFoundException("Estudiante no encontrado.");

        var alreadyEnrolled = await _enrollmentRepo.ExistsAsync(
            e => e.WorkshopId == dto.WorkshopId && e.StudentId == dto.StudentId
              && e.Status != EnrollmentStatus.Cancelled, ct);
        if (alreadyEnrolled)
            throw new InvalidOperationException("El estudiante ya está inscrito en este taller.");

        var enrollment = new Enrollment(dto.WorkshopId, dto.StudentId);
        await _enrollmentRepo.AddAsync(enrollment, ct);
        return MapToDto(enrollment);
    }

    public async Task<EnrollmentDto> ConfirmAsync(Guid id, CancellationToken ct = default)
    {
        var enrollment = await _enrollmentRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Inscripción no encontrada.");
        enrollment.Confirm();
        await _enrollmentRepo.UpdateAsync(enrollment, ct);
        return MapToDto(enrollment);
    }

    public async Task<EnrollmentDto> CancelAsync(Guid id, CancelEnrollmentDto dto, CancellationToken ct = default)
    {
        var enrollment = await _enrollmentRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Inscripción no encontrada.");
        enrollment.Cancel(dto.Reason);
        await _enrollmentRepo.UpdateAsync(enrollment, ct);
        return MapToDto(enrollment);
    }

    public async Task<EnrollmentDto> CompleteAsync(Guid id, CancellationToken ct = default)
    {
        var enrollment = await _enrollmentRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Inscripción no encontrada.");
        enrollment.Complete();
        await _enrollmentRepo.UpdateAsync(enrollment, ct);
        return MapToDto(enrollment);
    }

    private static EnrollmentDto MapToDto(Enrollment e) => new(
        e.Id, e.WorkshopId, e.Workshop?.Title,
        e.StudentId, e.Student?.FullName,
        e.EnrollmentDate, e.Status.ToString(), e.CancellationReason);
}

public class ScheduleService : IScheduleService
{
    private readonly IRepository<Schedule> _repo;
    private readonly IRepository<Workshop> _workshopRepo;

    public ScheduleService(IRepository<Schedule> repo,
                           IRepository<Workshop> workshopRepo)
    {
        _repo = repo;
        _workshopRepo = workshopRepo;
    }

    public async Task<IEnumerable<ScheduleDto>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct = default)
    {
        var list = await _repo.FindAsync(s => s.WorkshopId == workshopId, ct);
        return list.Select(MapToDto);
    }

    public async Task<IEnumerable<ScheduleDto>> GetUpcomingAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var list = await _repo.FindAsync(s => s.StartTime > now && !s.IsCancelled, ct);
        return list.OrderBy(s => s.StartTime).Select(MapToDto);
    }

    public async Task<ScheduleDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _repo.GetByIdAsync(id, ct);
        return s is null ? null : MapToDto(s);
    }

    public async Task<ScheduleDto> CreateAsync(CreateScheduleDto dto, CancellationToken ct = default)
    {
        var workshopExists = await _workshopRepo.ExistsAsync(w => w.Id == dto.WorkshopId, ct);
        if (!workshopExists) throw new KeyNotFoundException("Taller no encontrado.");

        var schedule = new Schedule(dto.WorkshopId, dto.StartTime, dto.EndTime, dto.Location);
        await _repo.AddAsync(schedule, ct);
        return MapToDto(schedule);
    }

    public async Task<ScheduleDto> RescheduleAsync(Guid id, RescheduleDto dto, CancellationToken ct = default)
    {
        var schedule = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Sesión no encontrada.");
        schedule.Reschedule(dto.NewStart, dto.NewEnd);
        await _repo.UpdateAsync(schedule, ct);
        return MapToDto(schedule);
    }

    public async Task CancelSessionAsync(Guid id, string note, CancellationToken ct = default)
    {
        var schedule = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Sesión no encontrada.");
        schedule.CancelSession(note);
        await _repo.UpdateAsync(schedule, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var schedule = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Sesión no encontrada.");
        await _repo.DeleteAsync(schedule, ct);
    }

    private static ScheduleDto MapToDto(Schedule s) => new(
        s.Id, s.WorkshopId, s.Workshop?.Title,
        s.StartTime, s.EndTime, s.Location,
        s.IsCancelled, s.CancellationNote,
        s.GetDuration().TotalMinutes);
}

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _repo;
    private readonly IRepository<Enrollment> _enrollmentRepo;

    public PaymentService(IRepository<Payment> repo,
                          IRepository<Enrollment> enrollmentRepo)
    {
        _repo = repo;
        _enrollmentRepo = enrollmentRepo;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync(CancellationToken ct = default)
    {
        var payments = await _repo.GetAllAsync(ct);
        return payments.Select(MapToDto);
    }

    public async Task<PaymentDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _repo.GetByIdAsync(id, ct);
        return p is null ? null : MapToDto(p);
    }

    public async Task<PaymentDto?> GetByEnrollmentAsync(Guid enrollmentId, CancellationToken ct = default)
    {
        var payments = await _repo.FindAsync(p => p.EnrollmentId == enrollmentId, ct);
        var payment = payments.FirstOrDefault();
        return payment is null ? null : MapToDto(payment);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        var enrollmentExists = await _enrollmentRepo.ExistsAsync(e => e.Id == dto.EnrollmentId, ct);
        if (!enrollmentExists) throw new KeyNotFoundException("Inscripción no encontrada.");

        var payment = new Payment(dto.EnrollmentId, dto.Amount, dto.Method);
        await _repo.AddAsync(payment, ct);
        return MapToDto(payment);
    }

    public async Task<PaymentDto> ConfirmPaymentAsync(Guid id, ConfirmPaymentDto dto, CancellationToken ct = default)
    {
        var payment = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Pago no encontrado.");
        payment.MarkAsPaid(dto.TransactionReference);
        await _repo.UpdateAsync(payment, ct);
        return MapToDto(payment);
    }

    public async Task<PaymentDto> RefundAsync(Guid id, string notes, CancellationToken ct = default)
    {
        var payment = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Pago no encontrado.");
        payment.Refund(notes);
        await _repo.UpdateAsync(payment, ct);
        return MapToDto(payment);
    }

    private static PaymentDto MapToDto(Payment p) => new(
        p.Id, p.EnrollmentId, p.Amount, p.Status.ToString(),
        p.Method.ToString(), p.TransactionReference, p.PaidAt, p.Notes);
}

public class ReportService : IReportService
{
    private readonly IRepository<Workshop> _workshopRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Instructor> _instructorRepo;
    private readonly IRepository<Enrollment> _enrollmentRepo;
    private readonly IRepository<Payment> _paymentRepo;

    public ReportService(IRepository<Workshop> workshopRepo,
                         IRepository<Student> studentRepo,
                         IRepository<Instructor> instructorRepo,
                         IRepository<Enrollment> enrollmentRepo,
                         IRepository<Payment> paymentRepo)
    {
        _workshopRepo = workshopRepo;
        _studentRepo = studentRepo;
        _instructorRepo = instructorRepo;
        _enrollmentRepo = enrollmentRepo;
        _paymentRepo = paymentRepo;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default)
    {
        var workshops = await _workshopRepo.GetAllAsync(ct);
        var students = await _studentRepo.GetAllAsync(ct);
        var instructors = await _instructorRepo.GetAllAsync(ct);
        var enrollments = await _enrollmentRepo.GetAllAsync(ct);
        var payments = await _paymentRepo.GetAllAsync(ct);

        return new DashboardStatsDto(
            workshops.Count(),
            workshops.Count(w => w.IsActive),
            students.Count(),
            instructors.Count(),
            enrollments.Count(),
            payments.Count(p => p.Status == PaymentStatus.Pending),
            payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount)
        );
    }

    public async Task<IEnumerable<WorkshopReportDto>> GetWorkshopReportAsync(CancellationToken ct = default)
    {
        var workshops = await _workshopRepo.GetAllAsync(ct);
        var enrollments = await _enrollmentRepo.GetAllAsync(ct);
        var payments = await _paymentRepo.GetAllAsync(ct);

        return workshops.Select(w =>
        {
            var wEnrollments = enrollments.Where(e => e.WorkshopId == w.Id).ToList();
            var wPayments = payments
                .Where(p => wEnrollments.Any(e => e.Id == p.EnrollmentId) && p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount);
            var enrolled = wEnrollments.Count(e => e.Status == EnrollmentStatus.Active);
            return new WorkshopReportDto(
                w.Id, w.Title,
                w.Instructor?.FullName ?? "Sin instructor",
                enrolled, w.MaxStudents,
                w.MaxStudents > 0 ? Math.Round((decimal)enrolled / w.MaxStudents * 100, 2) : 0,
                wPayments);
        });
    }
}
