namespace Bookify.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }

    public DateTime OccuredOnUtc { get; private set; }

    public string Type { get; private set; }

    
    // Instead of string this should be of type IDomainEvent and then use ToJson in Entity Configuration
    public string Content { get; private set; }

    public DateTime? ProcessedOnUtc { get;  set; }

    public string? Error { get;  set; }

    public OutboxMessage(Guid id, DateTime occuredOnUtc, string type, string content)
    {
        Id = id;
        OccuredOnUtc = occuredOnUtc;
        Type = type;
        Content = content;
    }
}
