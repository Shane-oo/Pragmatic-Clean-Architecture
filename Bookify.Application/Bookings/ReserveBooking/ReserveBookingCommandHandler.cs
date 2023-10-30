using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ReserveBooking;

public sealed class ReserveBookingCommandHandler: ICommandHandler<ReserveBookingCommand, Guid>
{
    #region Fields

    private readonly IApartmentRepository _apartmentRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly PricingService _pricingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    #endregion

    #region Construction

    public ReserveBookingCommandHandler(IApartmentRepository apartmentRepository,
                                        IBookingRepository bookingRepository,
                                        IUnitOfWork unitOfWork,
                                        IUserRepository userRepository,
                                        PricingService pricingService,
                                        IDateTimeProvider dateTimeProvider)
    {
        _apartmentRepository = apartmentRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _pricingService = pricingService;
        _dateTimeProvider = dateTimeProvider;
    }

    #endregion

    #region Public Methods

    public async Task<Result<Guid>> Handle(ReserveBookingCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(command.UserId), cancellationToken);

        if (user == null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }

        var apartment = await _apartmentRepository.GetByIdAsync(new ApartmentId(command.ApartmentId), cancellationToken);

        if (apartment == null)
        {
            return Result.Failure<Guid>(ApartmentErrors.NotFound);
        }

        var duration = DateRange.Create(command.StartDate, command.EndDate);

        if (await _bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }

        try
        {
            var booking = Booking.Reserve(apartment,
                                          new UserId(command.UserId),
                                          duration,
                                          _dateTimeProvider.UtcNow,
                                          _pricingService);

            _bookingRepository.Add(booking);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return booking.Id.Value;
        }
        catch(ConcurrencyException)
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }
    }

    #endregion
}
