using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

internal sealed class BookingRepository: Repository<Booking, BookingId>, IBookingRepository
{
    #region Fields

    private static readonly BookingStatus[] _activeBookingStatuses =
    {
        BookingStatus.Reserved,
        BookingStatus.Confirmed,
        BookingStatus.Completed
    };

    #endregion

    #region Construction

    public BookingRepository(ApplicationDbContext dbContext): base(dbContext)
    {
    }

    #endregion

    #region Public Methods

    public async Task<bool> IsOverlappingAsync(Apartment apartment, DateRange duration, CancellationToken cancellationToken = default)
    {
        return await _dbContext
                     .Set<Booking>()
                     .AnyAsync(b =>
                                   b.ApartmentId == apartment.Id &&
                                   b.Duration.Start <= duration.End &&
                                   b.Duration.End >= duration.Start &&
                                   _activeBookingStatuses.Contains(b.Status),
                               cancellationToken);
    }

    #endregion
}
