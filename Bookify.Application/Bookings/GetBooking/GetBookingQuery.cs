using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Bookings.GetBooking;

public class GetBookingQuery: IQuery<BookingResponse>
{
    #region Properties

    public Guid Id { get; set; }

    #endregion
}
