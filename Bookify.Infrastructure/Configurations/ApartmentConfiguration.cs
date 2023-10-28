using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class ApartmentConfiguration: IEntityTypeConfiguration<Apartment>
{
    #region Public Methods

    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(b => b.Id)
               .HasConversion(id => id.Value, value => new ApartmentId(value));

        builder.OwnsOne(a => a.Address);

        builder.Property(a => a.Name)
               .HasMaxLength(200)
               .HasConversion(n => n.Value, value => new Name(value));

        builder.Property(a => a.Description)
               .HasMaxLength(2000)
               .HasConversion(n => n.Value, value => new Description(value));

        builder.OwnsOne(a => a.Price, priceBuilder =>
                                      {
                                          priceBuilder.Property(m => m.Currency)
                                                      .HasConversion(c => c.Code, code => Currency.FromCode(code));
                                      });

        builder.OwnsOne(a => a.CleaningFee, priceBuilder =>
                                            {
                                                priceBuilder.Property(m => m.Currency)
                                                            .HasConversion(c => c.Code, code => Currency.FromCode(code));
                                            });


        builder.Ignore(a => a.Amenities);// Ignore for now

        builder.HasMany(a => a.Bookings)
               .WithOne(b => b.Apartment)
               .HasForeignKey(b => b.ApartmentId);
        
        // Concurrency
        builder.Property<uint>("Version").IsRequired();
    }

    #endregion
}
