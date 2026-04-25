using Ceramix.Application.DTOs;

namespace Ceramix.Application.Interfaces;

public interface IWorkshopService
{
    Task<IEnumerable<WorkshopDto>> GetAllAsync(CancellationToken ct = default);
    Task<WorkshopDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<WorkshopDto>> GetByInstructorAsync(Guid instructorId, CancellationToken ct = default);
    Task<IEnumerable<WorkshopDto>> SearchByTitleAsync(string title, CancellationToken ct = default);
    Task<WorkshopDto> CreateAsync(CreateWorkshopDto dto, CancellationToken ct = default);
    Task<WorkshopDto> UpdateAsync(Guid id, UpdateWorkshopDto dto, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IInstructorService
{
    Task<IEnumerable<InstructorDto>> GetAllAsync(CancellationToken ct = default);
    Task<InstructorDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InstructorDto> CreateAsync(CreateInstructorDto dto, CancellationToken ct = default);
    Task<InstructorDto> UpdateAsync(Guid id, UpdateInstructorDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync(CancellationToken ct = default);
    Task<StudentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<StudentDto>> SearchAsync(string term, CancellationToken ct = default);
    Task<StudentDto> CreateAsync(CreateStudentDto dto, CancellationToken ct = default);
    Task<StudentDto> UpdateAsync(Guid id, UpdateStudentDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IEnrollmentService
{
    Task<IEnumerable<EnrollmentDto>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<EnrollmentDto>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct = default);
    Task<IEnumerable<EnrollmentDto>> GetByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<EnrollmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<EnrollmentDto> CreateAsync(CreateEnrollmentDto dto, CancellationToken ct = default);
    Task<EnrollmentDto> ConfirmAsync(Guid id, CancellationToken ct = default);
    Task<EnrollmentDto> CancelAsync(Guid id, CancelEnrollmentDto dto, CancellationToken ct = default);
    Task<EnrollmentDto> CompleteAsync(Guid id, CancellationToken ct = default);
}

public interface IScheduleService
{
    Task<IEnumerable<ScheduleDto>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct = default);
    Task<IEnumerable<ScheduleDto>> GetUpcomingAsync(CancellationToken ct = default);
    Task<ScheduleDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ScheduleDto> CreateAsync(CreateScheduleDto dto, CancellationToken ct = default);
    Task<ScheduleDto> RescheduleAsync(Guid id, RescheduleDto dto, CancellationToken ct = default);
    Task CancelSessionAsync(Guid id, string note, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllAsync(CancellationToken ct = default);
    Task<PaymentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaymentDto?> GetByEnrollmentAsync(Guid enrollmentId, CancellationToken ct = default);
    Task<PaymentDto> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default);
    Task<PaymentDto> ConfirmPaymentAsync(Guid id, ConfirmPaymentDto dto, CancellationToken ct = default);
    Task<PaymentDto> RefundAsync(Guid id, string notes, CancellationToken ct = default);
}

public interface IReportService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default);
    Task<IEnumerable<WorkshopReportDto>> GetWorkshopReportAsync(CancellationToken ct = default);
}
