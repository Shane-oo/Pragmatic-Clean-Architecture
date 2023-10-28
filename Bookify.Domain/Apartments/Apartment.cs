using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Apartments;

public sealed class Apartment: Entity<ApartmentId>
{
    #region Properties

    public Apartment(ApartmentId id, Name name, Description description, Address address, Money price, Money cleaningFee, DateTime? lastBookedOnUtc): base(id)
    {
        Name = name;
        Description = description;
        Address = address;
        Price = price;
        CleaningFee = cleaningFee;
        LastBookedOnUtc = lastBookedOnUtc;
    }

    private Apartment()
    {
    }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public Address Address { get; private set; }

    public Money Price { get; private set; }

    public Money CleaningFee { get; private set; }

    public List<Amenity> Amenities { get; private set; } = new();

    public DateTime? LastBookedOnUtc { get; internal set; }

    public List<Booking> Bookings { get; private set; }

    #endregion

    #region Construction

    public Apartment(ApartmentId id): base(id)
    {
    }

    #endregion
}
