using Bookify.Application.Apartments.SearchApartments;
using Bookify.Domain.Apartments;

namespace Bookify.Infrastructure.Data.Queries.Apartments;

public class SearchApartmentsDbQuery: ISearchApartmentsDbQuery
{
    #region Fields

    private bool _asNoTracking;

    private readonly ApplicationDbContext _dbContext;

    // might need a Converter for DateOnly types for entity framework core 7
    private DateOnly _endDate;
    private DateOnly _startDate;

    #endregion

    #region Construction

    public SearchApartmentsDbQuery(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion

    #region Public Methods

    public async Task<List<Apartment>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        //var query = _dbContext.Apartments.Where(a=>a.StartDate >= _startDate && a.EndDate <= _endDate));

        //if(_asNoTracking){
        //query = query.AsNoTracking();
        //}
        //return await query.ToListAsync();

        return new List<Apartment> { new(ApartmentId.New()) };
    }

    public ISearchApartmentsDbQuery WithNoTracking()
    {
        _asNoTracking = true;
        return this;
    }

    public ISearchApartmentsDbQuery WithParams(DateOnly startDate, DateOnly endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
        return this;
    }

    #endregion
}
