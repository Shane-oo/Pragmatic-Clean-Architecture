using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(UserId UserId): IDomainEvent;
