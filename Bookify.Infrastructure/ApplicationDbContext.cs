using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext: DbContext, IUnitOfWork
{
    #region Fields

    private readonly IDateTimeProvider _dateTimeProvider;

    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
                                                                             {
                                                                                 TypeNameHandling = TypeNameHandling.All
                                                                             };

    private readonly IPublisher _publisher;

    #endregion

    #region Properties

    public DbSet<Apartment> Apartments { get; set; }

    public DbSet<Booking> Bookings { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    //public DbSet<Review> Reviews { get; set; }

    public DbSet<User> Users { get; set; }

    #endregion

    #region Construction

    public ApplicationDbContext(DbContextOptions options, IPublisher publisher, IDateTimeProvider dateTimeProvider): base(options)
    {
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
    }

    #endregion

    #region Private Methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        var domainEvents = ChangeTracker
                           .Entries<IEntity>()
                           .Select(e => e.Entity)
                           .SelectMany(e =>
                                       {
                                           var domainEvents = e.GetDomainEvents();

                                           e.ClearDomainEvents();

                                           return domainEvents;
                                       })
                           .Select(domainEvent => new OutboxMessage(Guid.NewGuid(),
                                                                    _dateTimeProvider.UtcNow,
                                                                    domainEvent.GetType().Name,
                                                                    JsonConvert.SerializeObject(domainEvent, _jsonSerializerSettings)))
                           .ToList();
        AddRange(domainEvents);
    }

    #endregion

    #region Public Methods

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        try
        {
            AddDomainEventsAsOutboxMessages();

            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            return result;
        }
        catch(DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occured.", ex);
        }
    }

    #endregion
}
