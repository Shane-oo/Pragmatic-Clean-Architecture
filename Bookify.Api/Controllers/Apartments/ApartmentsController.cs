using Bookify.Application.Apartments.SearchApartments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Apartments;

[Route("api/[controller]")]
[ApiController]
public class ApartmentsController: ControllerBase
{
    #region Fields

    private readonly ISender _sender;

    #endregion

    #region Construction

    public ApartmentsController(ISender sender)
    {
        _sender = sender;
    }

    #endregion

    #region Public Methods

    [HttpGet]
    public async Task<IActionResult> SearchApartments([FromQuery] SearchApartmentsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }

    #endregion
}
