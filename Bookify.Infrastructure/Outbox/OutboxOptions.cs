namespace Bookify.Infrastructure.Outbox;

public class OutboxOptions
{
    #region Properties

    public int BatchSize { get; init; }

    public int IntervalInSeconds { get; init; }

    #endregion
}
