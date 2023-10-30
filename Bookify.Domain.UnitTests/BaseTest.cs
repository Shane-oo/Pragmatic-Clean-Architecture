using Bookify.Domain.Abstractions;

namespace Bookify.Domain.UnitTests;

public abstract class BaseTest
{
    #region Public Methods

    public static T AssertDomainEventWasPublished<T>(IEntity entity)
        where T : IDomainEvent
    {
        var domainEvent = entity.GetDomainEvents().OfType<T>().SingleOrDefault();

        if (domainEvent == null)
        {
            throw new Exception($"{typeof(T).Name} was not published");
        }

        return domainEvent;
    }

    #endregion
}
