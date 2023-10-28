using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class UserConfiguration: IEntityTypeConfiguration<User>
{
    #region Public Methods

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .HasConversion(id => id.Value, value => new UserId(value));

        builder.Property(u => u.FirstName)
               .HasMaxLength(200)
               .HasConversion(firstName => firstName.Value, value => new FirstName(value));

        builder.Property(u => u.LastName)
               .HasMaxLength(200)
               .HasConversion(firstName => firstName.Value, value => new LastName(value));

        builder.Property(u => u.Email)
               .HasMaxLength(400)
               .HasConversion(firstName => firstName.Value, value => new Domain.Users.Email(value));


        builder.HasMany(u => u.Bookings)
               .WithOne(b => b.User)
               .HasForeignKey(b => b.UserId);

        builder.HasIndex(u => u.Email).IsUnique();
    }

    #endregion
}
