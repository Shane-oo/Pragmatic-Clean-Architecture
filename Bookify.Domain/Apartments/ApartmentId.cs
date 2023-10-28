namespace Bookify.Domain.Apartments;

public record ApartmentId(Guid Value)
{
    #region Public Methods

    public static ApartmentId New()
    {
        return new ApartmentId(Guid.NewGuid());
    }

    #endregion
}
