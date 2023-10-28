using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;
using Bookify.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext: DbContext, IUnitOfWork
{
    #region Fields

    private readonly IPublisher _publisher;

    #endregion

    #region Properties

    public DbSet<Apartment> Apartments { get; set; }

    public DbSet<Booking> Bookings { get; set; }

    //public DbSet<Review> Reviews { get; set; }

    public DbSet<User> Users { get; set; }

    #endregion

    #region Construction

    public ApplicationDbContext(DbContextOptions options, IPublisher publisher): base(options)
    {
        _publisher = publisher;
    }

    #endregion

    #region Private Methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
                           .Entries<IEntity>()
                           .Select(entry => entry.Entity)
                           .SelectMany(entity =>
                                       {
                                           var domainEvents = entity.GetDomainEvents();

                                           entity.ClearDomainEvents();
                                           return domainEvents;
                                       })
                           .ToList();

        foreach(var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }

    #endregion

    #region Public Methods

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            // not robust
            await PublishDomainEventsAsync();

            return result;
        }
        catch(DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occured.", ex);
        }
    }

    #endregion
}
