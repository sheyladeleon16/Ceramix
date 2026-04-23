namespace Ceramix.Domain.Entities;

public class Session
{
    public Guid Id { get; set; }
    public string Topic { get; set; }
    public DateTime Date { get; set; }

    public Session(string topic, DateTime date)
    {
        Id = Guid.NewGuid();
        Topic = topic;
        Date = date;
    }
}