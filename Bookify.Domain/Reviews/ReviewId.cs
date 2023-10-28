namespace Bookify.Domain.Reviews;

public record ReviewId(Guid Value)
{
    #region Public Methods

    public static ReviewId New()
    {
        return new ReviewId(Guid.NewGuid());
    }

    #endregion
}

