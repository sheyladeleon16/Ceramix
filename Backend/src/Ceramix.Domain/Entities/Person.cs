namespace Ceramix.Domain.Entities;

public abstract class Person : BaseEntity
{
    public string FullName { get; protected set; }
    public string Email { get; protected set; }
    public string Phone { get; protected set; }
    public DateTime DateOfBirth { get; protected set; }

    protected Person(string fullName, string email, string phone, DateTime dateOfBirth)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? string.Empty;
        DateOfBirth = dateOfBirth;
    }

    public int GetAge() => DateTime.UtcNow.Year - DateOfBirth.Year;

    public void UpdateContact(string email, string phone)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? string.Empty;
        MarkAsUpdated();
    }
}
