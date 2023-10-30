using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using FakeItEasy;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Bookify.Application.UnitTests.Bookings;

[TestClass]
public class ReserveBooking
{
    #region Public Methods

    [TestMethod]
    public async Task Handle_Should_ReturnFailure_WhenUserIsNull()
    {
        // arrange
        var command = A.Fake<ReserveBookingCommand>();
        var cancellationToken = new CancellationToken();
        
        var userRepository = new Fake<IUserRepository>().FakedObject;
        A.CallTo(() => userRepository.GetByIdAsync(A<UserId>._, A<CancellationToken>._)).Returns((User)null!);

        var sut = new ReserveBookingCommandHandler(
                                                   new Fake<IApartmentRepository>().FakedObject,
                                                   new Fake<IBookingRepository>().FakedObject,
                                                   new Fake<IUnitOfWork>().FakedObject,
                                                   userRepository,
                                                   new Fake<PricingService>().FakedObject,
                                                   new Fake<IDateTimeProvider>().FakedObject
                                                  );

        // act
        var result = await sut.Handle(command, cancellationToken);

        // assert
        Assert.IsTrue(result.Error == UserErrors.NotFound, "Not Found Should be returned when user is null");
    }

    #endregion
}
