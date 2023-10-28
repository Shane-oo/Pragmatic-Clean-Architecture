using Bookify.Domain.Apartments;

namespace Bookify.Infrastructure.Repositories;

internal sealed class ApartmentRepository: Repository<Apartment, ApartmentId>, IApartmentRepository
{
    #region Construction

    public ApartmentRepository(ApplicationDbContext dbContext): base(dbContext)
    {
    }

    #endregion
}
