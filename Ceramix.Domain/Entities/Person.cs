namespace Ceramix.Domain.Entities;

public abstract class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    protected Person(string name, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }
}