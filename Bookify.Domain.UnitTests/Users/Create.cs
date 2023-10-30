using Bookify.Domain.Users;
using Bookify.Domain.Users.Events;
using FakeItEasy;

namespace Bookify.Domain.UnitTests.Users;

[TestClass]
public class Create: BaseTest
{
    #region Public Methods

    [TestMethod]
    public void Create_Should_Raise_UserCreatedDomainEvent()
    {
        // arrange
        var firstName = A.Fake<FirstName>();
        var lastName = A.Fake<LastName>();
        var email = A.Fake<Email>();

        // act
        var user = User.Create(firstName, lastName, email);

        // assert
        var userCreatedDomainEvent = AssertDomainEventWasPublished<UserCreatedDomainEvent>(user);

        Assert.IsTrue(userCreatedDomainEvent.UserId.Equals(user.Id));
    }

    #endregion
}
