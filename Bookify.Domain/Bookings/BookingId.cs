namespace Bookify.Domain.Bookings;

public record BookingId(Guid Value)
{
    #region Public Methods

    public static BookingId New()
    {
        return new BookingId(Guid.NewGuid());
    }

    #endregion
}
