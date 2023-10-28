using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class BookingConfiguration: IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Id)
               .HasConversion(id => id.Value, value => new BookingId(value));


        builder.Property(u => u.UserId)
               .HasConversion(id => id.Value, value => new UserId(value));


        builder.Property(u => u.ApartmentId)
               .HasConversion(id => id.Value, value => new ApartmentId(value));

        builder.OwnsOne(b => b.PriceForPeriod, priceBuilder =>
                                               {
                                                   priceBuilder.Property(m => m.Currency)
                                                               .HasConversion(c => c.Code, code => Currency.FromCode(code));
                                               });

        builder.OwnsOne(b => b.CleaningFee, priceBuilder =>
                                               {
                                                   priceBuilder.Property(m => m.Currency)
                                                               .HasConversion(c => c.Code, code => Currency.FromCode(code));
                                               });
        
        builder.OwnsOne(b => b.AmenitiesUpCharge, priceBuilder =>
                                            {
                                                priceBuilder.Property(m => m.Currency)
                                                            .HasConversion(c => c.Code, code => Currency.FromCode(code));
                                            });
        
        builder.OwnsOne(b => b.TotalPrice, priceBuilder =>
                                                  {
                                                      priceBuilder.Property(m => m.Currency)
                                                                  .HasConversion(c => c.Code, code => Currency.FromCode(code));
                                                  });

       // builder.OwnsOne(b => b.Duration);  EF Core 8
       builder.Ignore(b => b.Duration);
    }
}
