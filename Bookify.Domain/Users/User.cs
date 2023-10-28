using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;

public sealed class User: Entity<UserId>
{
    #region Properties

    public FirstName FirstName { get; private set; }

    public LastName LastName { get; private set; }

    public Email Email { get; private set; }
    
    public List<Booking> Bookings { get; private set; }

    #endregion

    #region Construction

    private User(UserId id, FirstName firstName, LastName lastName, Email email): base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
    
    private User(){}

    #endregion

    #region Public Methods

    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(UserId.New(), firstName, lastName, email);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        return user;
    }

    #endregion
}
