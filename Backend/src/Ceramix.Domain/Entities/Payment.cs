using Ceramix.Domain.Enums;

namespace Ceramix.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid EnrollmentId { get; private set; }
    public Enrollment? Enrollment { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? TransactionReference { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? Notes { get; private set; }

    public Payment(Guid enrollmentId, decimal amount, PaymentMethod method)
    {
        EnrollmentId = enrollmentId;
        Amount = amount > 0 ? amount : throw new ArgumentException("Amount must be positive.");
        Method = method;
        Status = PaymentStatus.Pending;
    }

    public void MarkAsPaid(string transactionReference)
    {
        Status = PaymentStatus.Paid;
        TransactionReference = transactionReference;
        PaidAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void MarkAsFailed(string notes)
    {
        Status = PaymentStatus.Failed;
        Notes = notes;
        MarkAsUpdated();
    }

    public void Refund(string notes)
    {
        if (Status != PaymentStatus.Paid)
            throw new InvalidOperationException("Solo se pueden reembolsar pagos completados.");
        Status = PaymentStatus.Refunded;
        Notes = notes;
        MarkAsUpdated();
    }

    public override string GetSummary() =>
        $"Pago: {Amount:C} | {Method} | {Status} | {PaidAt?.ToString("dd/MM/yyyy") ?? "Pendiente"}";
}
