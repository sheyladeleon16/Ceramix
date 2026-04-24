namespace Ceramix.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;
    public void SoftDelete()
    {
        IsDeleted = true;
        MarkAsUpdated();
    }

    public abstract string GetSummary();
}